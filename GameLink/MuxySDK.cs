using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MuxyGameLink.Imports;

namespace MuxyGameLink
{
    public class SDK
    {
        public static String STATE_TARGET_CHANNEL = "channel";
        public static String STATE_TARGET_EXTENSION = "extension";

        public String ConnectionAddress(Stage Stage)
        {
            IntPtr Ptr = Imported.ProjectionWebsocketConnectionURL(this.ClientId, (Int32)Stage, "csharp", 0, 0, 1);
            return NativeString.StringFromUTF8AndDeallocate(Ptr);
        }
        
        public SDK(String ClientId)
        {
            this.Instance = Imported.Make();

            this.ClientId = ClientId;

            OnDatastreamHandles = new Dictionary<UInt32, GCHandle>();
            OnTwitchPurchaseHandles = new Dictionary<UInt32, GCHandle>();
            OnStateUpdateHandles = new Dictionary<UInt32, GCHandle>();
            OnPollUpdateHandles = new Dictionary<UInt32, GCHandle>();
            OnConfigUpdateHandles = new Dictionary<UInt32, GCHandle>();
        }

        ~SDK()
        {
            Imported.Kill(this.Instance);
        }

        #region Authentication and User Management
        public bool IsAuthenticated()
        {
            return Imported.IsAuthenticated(this.Instance);
        }

        public delegate void AuthenticationCallback(AuthenticationResponse Payload);
        public UInt16 AuthenticateWithRefreshToken(string RefreshToken, AuthenticationCallback Callback)
        {
            AuthenticateResponseDelegate WrapperCallback = ((UserData, AuthResp) =>
            {
                AuthenticationResponse Response = new AuthenticationResponse(AuthResp);
                Callback(Response);

                GCHandle WHandle = GCHandle.FromIntPtr(UserData);
                WHandle.Free();
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            return Imported.AuthenticateWithRefreshToken(this.Instance, this.ClientId, RefreshToken, WrapperCallback, ((IntPtr)Handle));
        }

        public UInt16 AuthenticateWithPIN(string PIN, AuthenticationCallback Callback)
        {
            AuthenticateResponseDelegate WrapperCallback = ((UserData, AuthResp) =>
            {
                AuthenticationResponse Response = new AuthenticationResponse(AuthResp);
                Callback(Response);

                GCHandle WHandle = GCHandle.FromIntPtr(UserData);
                WHandle.Free();
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            return Imported.AuthenticateWithPIN(this.Instance, this.ClientId, PIN, WrapperCallback, ((IntPtr)Handle));
        }

        public User User
        {
            get
            {
                if (!this.IsAuthenticated())
                {
                    return null;
                }

                if (this.CachedUserInstance != null)
                {
                    return this.CachedUserInstance;
                }

                this.CachedUserInstance = new User(Imported.GetSchemaUser(this.Instance));
                return this.CachedUserInstance;
            }
        }
        #endregion

        #region Network Interface
        public bool ReceiveMessage(string Message)
        {
            return Imported.ReceiveMessage(this.Instance, Message, (uint)Message.Length);
        }

        public delegate void PayloadCallback(string Payload);
        public void ForEachPayload(PayloadCallback Callback)
        {
            PayloadDelegate WrapperCallback = ((UserData, Payload) =>
            {
                IntPtr ptr = Imported.Payload_GetData(Payload);
                UInt64 len = Imported.Payload_GetSize(Payload);

                string str = NativeString.StringFromUTF8(ptr, ((int)len));
                Callback(str);
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            Imported.ForeachPayload(this.Instance, WrapperCallback, IntPtr.Zero);
            Handle.Free();
        }
        #endregion

        #region State Operations
        public UInt16 SetState(string Target, string Json)
        {
            return Imported.SetState(this.Instance, Target, Json);
        }

        public delegate void GetStateCallback(StateResponse Response);
        public UInt16 GetState(string Target, GetStateCallback Callback)
        {
            StateGetDelegate WrapperCallback = ((UserData, StateResp) =>
            {
                StateResponse Response = new StateResponse(StateResp);
                Callback(Response);

                GCHandle WHandle = GCHandle.FromIntPtr(UserData);
                WHandle.Free();
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            return Imported.GetState(this.Instance, Target, WrapperCallback, ((IntPtr)Handle));
        }

        public UInt16 UpdateStateWithInteger(string Target, string Operation, string Path, Int64 Value)
        {
            return Imported.UpdateStateWithInteger(this.Instance, Target, Operation, Path, Value);
        }

        public UInt16 UpdateStateWithDouble(string Target, string Operation, string Path, Double Value)
        {
            return Imported.UpdateStateWithDouble(this.Instance, Target, Operation, Path, Value);
        }

        public UInt16 UpdateStateWithString(string Target, string Operation, string Path, string Value)
        {
            return Imported.UpdateStateWithString(this.Instance, Target, Operation, Path, Value);
        }

        public UInt16 UpdateStateWithLiteral(string Target, string Operation, string Path, string JsonLiteral)
        {
            return Imported.UpdateStateWithLiteral(this.Instance, Target, Operation, Path, JsonLiteral);
        }

        public UInt16 UpdateStateWithNull(string Target, string Operation, string Path)
        {
            return Imported.UpdateStateWithNull(this.Instance, Target, Operation, Path);
        }

        public UInt16 SubscribeToStateUpdates(string Target)
        {
            return Imported.SubscribeToStateUpdates(this.Instance, Target);
        }

        public UInt16 UnsubscribeFromStateUpdates(string Target)
        {
            return Imported.UnsubscribeFromStateUpdates(this.Instance, Target);
        }

        public delegate void UpdateStateCallback(StateUpdate Response);
        public UInt32 OnStateUpdate(UpdateStateCallback Callback)
        {
            StateUpdateDelegate WrapperCallback = ((UserData, Update) =>
            {
                StateUpdate Response = new StateUpdate(Update);
                Callback(Response);
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            UInt32 Result = Imported.OnStateUpdate(this.Instance, WrapperCallback, IntPtr.Zero);

            OnStateUpdateHandles.Add(Result, Handle);
            return Result;
        }

        public void DetachOnStateUpdate(UInt32 Handle)
        {
            Imported.DetachOnStateUpdate(this.Instance, Handle);
            GCHandle GC = OnStateUpdateHandles[Handle];
            if (GC != null)
            {
                GC.Free();
                OnStateUpdateHandles.Remove(Handle);
            }
        }
        #endregion

        #region Configuration Operations
        public UInt16 SetChannelConfig(string JsonLiteral)
        {
            return Imported.SetChannelConfig(this.Instance, JsonLiteral);
        }

        public delegate void GetChannelCallback(ConfigResponse Response);
        public UInt16 GetChannelConfig(string Target, GetChannelCallback Callback)
        {
            ConfigGetDelegate WrapperCallback = ((UserData, ConfigResp) =>
            {
                ConfigResponse Response = new ConfigResponse(ConfigResp);
                Callback(Response);

                GCHandle InternalHandle = GCHandle.FromIntPtr(UserData);
                InternalHandle.Free();
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            return Imported.GetConfig(this.Instance, Target, WrapperCallback, ((IntPtr)Handle));
        }

        public UInt16 UpdateConfigWithInteger(string Target, string Operation, string Path, Int64 Value)
        {
            return Imported.UpdateChannelConfigWithInteger(this.Instance, Operation, Path, Value);
        }

        public UInt16 UpdateConfigWithDouble(string Target, string Operation, string Path, Double Value)
        {
            return Imported.UpdateChannelConfigWithDouble(this.Instance, Operation, Path, Value);
        }

        public UInt16 UpdateConfigWithString(string Target, string Operation, string Path, string Value)
        {
            return Imported.UpdateChannelConfigWithString(this.Instance, Operation, Path, Value);
        }

        public UInt16 UpdateConfigWithLiteral(string Target, string Operation, string Path, string JsonLiteral)
        {
            return Imported.UpdateChannelConfigWithLiteral(this.Instance, Operation, Path, JsonLiteral);
        }

        public UInt16 UpdateConfigWithNull(string Target, string Operation, string Path)
        {
            return Imported.UpdateChannelConfigWithNull(this.Instance, Operation, Path);
        }

        public UInt16 SubscribeToConfigurationChanges(string Target)
        {
            return Imported.SubscribeToConfigurationChanges(this.Instance, Target);
        }

        public UInt16 UnsubscribeFromConfigurationChanges(string Target)
        {
            return Imported.UnsubscribeFromConfigurationChanges(this.Instance, Target);
        }

        public delegate void UpdateConfigCallback(ConfigUpdate Response);
        public UInt32 OnConfigUpdate(UpdateConfigCallback Callback)
        {
            ConfigUpdateDelegate WrapperCallback = ((UserData, Update) =>
            {
                ConfigUpdate Response = new ConfigUpdate(Update);
                Callback(Response);
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            UInt32 Result = Imported.OnConfigUpdate(this.Instance, WrapperCallback, IntPtr.Zero);

            OnConfigUpdateHandles.Add(Result, Handle);
            return Result;
        }

        public void DetachOnConfigUpdate(UInt32 Handle)
        {
            Imported.DetachOnConfigUpdate(this.Instance, Handle);
            GCHandle GC = OnConfigUpdateHandles[Handle];
            if (GC != null)
            {
                GC.Free();
                OnConfigUpdateHandles.Remove(Handle);
            }
        }
        #endregion

        #region Broadcasts
        public UInt16 SendBroadcast(string Target, string Json)
        {
            return Imported.SendBroadcast(this.Instance, Target, Json);
        }
        #endregion

        #region Datastream
        public UInt16 SubscribeToDatastream()
        {
            return Imported.SubscribeToDatastream(this.Instance);
        }

        public UInt16 UnsubscribeFromDatastream()
        {
            return Imported.UnsubscribeFromDatastream(this.Instance);
        }

        public delegate void DatastreamCallback(DatastreamUpdate Update);
        public UInt32 OnDatastream(DatastreamCallback Callback)
        {
            DatastreamUpdateDelegate WrapperCallback = ((IntPtr UserData, Imports.Schema.DatastreamUpdate Update) =>
            {
                DatastreamUpdate Response = new DatastreamUpdate(Update);
                Callback(Response);
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            UInt32 Result = Imported.OnDatastream(this.Instance, WrapperCallback, IntPtr.Zero);

            OnDatastreamHandles.Add(Result, Handle);
            return Result;
        }

        public void DetachOnDatastream(UInt32 Handle)
        {
            Imported.DetachOnDatastream(this.Instance, Handle);
            GCHandle GC = OnDatastreamHandles[Handle];
            if (GC != null)
            {
                GC.Free();
                OnDatastreamHandles.Remove(Handle);
            }
        }
        #endregion

        #region Twitch Purchases
        public UInt16 SubscribeToSKU(string SKU)
        {
            return Imported.SubscribeToSKU(this.Instance, SKU);
        }

        public UInt16 UnsubscribeFromSKU(string SKU)
        {
            return Imported.UnsubscribeFromSKU(this.Instance, SKU);
        }

        public UInt16 SubscribeToAllPurchases()
        {
            return Imported.SubscribeToAllPurchases(this.Instance);
        }

        public UInt16 UnsubscribeFromAllPurchases()
        {
            return Imported.UnsubscribeFromAllPurchases(this.Instance);
        }

        public delegate void TwitchPurchaseBitsCallback(TwitchPurchaseBits Purchase);
        public UInt32 OnTwitchPurchaseBits(TwitchPurchaseBitsCallback Callback)
        {
            TwitchPurchaseBitsResponseDelegate WrapperCallback = ((IntPtr UserData, Imports.Schema.TwitchPurchaseBitsResponse Response) =>
            {
                TwitchPurchaseBits Converted = new TwitchPurchaseBits(Response);
                Callback(Converted);
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            UInt32 Result = Imported.OnTwitchPurchaseBits(this.Instance, WrapperCallback, IntPtr.Zero);

            OnTwitchPurchaseHandles.Add(Result, Handle);
            return Result;
        }

        public void DetachOnTwitchPurchaseBits(UInt32 Handle)
        {
            Imported.DetachOnTwitchPurchaseBits(this.Instance, Handle);
            GCHandle GC = OnTwitchPurchaseHandles[Handle];
            if (GC != null)
            {
                GC.Free();
                OnTwitchPurchaseHandles.Remove(Handle);
            }
        }
        #endregion

        #region Polling
        public UInt16 CreatePoll(String PollId, String Prompt, List<String> Options)
        {
            return Imported.CreatePoll(this.Instance, PollId, Prompt, Options.ToArray(), (UInt32)Options.Count);
        }

        public UInt16 SubscribeToPoll(String PollId)
        {
            return Imported.SubscribeToPoll(this.Instance, PollId);
        }

        public UInt16 UnsubscribeFromPoll(String PollId)
        {
            return Imported.UnsubscribeFromPoll(this.Instance, PollId);
        }

        public UInt16 DeletePoll(String PollId)
        {
            return Imported.DeletePoll(this.Instance, PollId);
        }

        public delegate void GetPollCallback(GetPollResponse Response);
        public UInt16 GetPoll(String PollId, GetPollCallback Callback)
        {
            GetPollResponseDelegate WrapperCallback = ((IntPtr UserData, Imports.Schema.GetPollResponse Response) =>
            {
                GetPollResponse Converted = new GetPollResponse(Response);
                Callback(Converted);
            });

            UInt16 Result = Imported.GetPoll(this.Instance, PollId, WrapperCallback, IntPtr.Zero);
            return Result;
        }

        public delegate void PollUpdateResponseCallback(PollUpdateResponse PResp);
        public UInt32 OnPollUpdate(PollUpdateResponseCallback Callback)
        {
            PollUpdateResponseDelegate WrapperCallback = ((IntPtr UserData, Imports.Schema.PollUpdateResponse PResp) =>
            {
                PollUpdateResponse Response = new PollUpdateResponse(PResp);
                Callback(Response);
            });

            GCHandle Handle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            UInt32 Result = Imported.OnPollUpdate(this.Instance, WrapperCallback, IntPtr.Zero);

            OnPollUpdateHandles.Add(Result, Handle);
            return Result;
        }

        public void DetachOnPollUpdate(UInt32 Handle)
        {
            Imported.DetachOnPollUpdate(this.Instance, Handle);
            GCHandle GC = OnPollUpdateHandles[Handle];
            if (GC != null)
            {
                GC.Free();
                OnPollUpdateHandles.Remove(Handle);
            }
        }
        #endregion

        #region Debugging
        public delegate void OnDebugMessageCallback(String Message);
        public void OnDebugMessage(OnDebugMessageCallback Callback)
        {
            DebugMessageDelegate WrapperCallback = ((IntPtr UserData, String Message) =>
            {
                Callback(Message);
            });

            OnDebugMessageHandle = GCHandle.Alloc(WrapperCallback, GCHandleType.Pinned);
            Imported.OnDebugMessage(this.Instance, WrapperCallback, IntPtr.Zero);
        }  
        public void DetachOnDebugMessage()
        {
            Imported.DetachOnDebugMessage(this.Instance);
            if (OnDebugMessageHandle != null)
            {
                OnDebugMessageHandle.Free();
            }
        }
        #endregion

        #region Members
        public String ClientId {get; set;}

        private SDKInstance Instance;
        private User CachedUserInstance;

        private GCHandle OnDebugMessageHandle;
        private Dictionary<UInt32, GCHandle> OnDatastreamHandles;
        private Dictionary<UInt32, GCHandle> OnTwitchPurchaseHandles;
        private Dictionary<UInt32, GCHandle> OnStateUpdateHandles;
        private Dictionary<UInt32, GCHandle> OnPollUpdateHandles;
        private Dictionary<UInt32, GCHandle> OnConfigUpdateHandles;
        #endregion
    }
}
