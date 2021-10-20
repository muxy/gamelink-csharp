using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MuxyGameLink.Imports;

namespace MuxyGameLink
{
    public enum Stage
    {
        Production = 0,
        Sandbox = 1
    };

    public class Error
    {
        public Error(NativeError Obj)
        {
            this._Title = NativeString.StringFromUTF8(Imported.Error_GetTitle(Obj));
            this._Detail = NativeString.StringFromUTF8(Imported.Error_GetDetail(Obj));
        }

        private String _Title;
        public String Title
        {
            get => _Title;
        }

        private String _Detail;
        public String Detail
        {
            get => _Detail;
        }

        public override string ToString()
        {
            return String.Format("Muxy GameLink Error: {0} ({1})", Title, Detail);
        }
    }

    public class User
    {
        public User(Imports.Schema.User User)
        {
            this._RefreshToken = NativeString.StringFromUTF8(Imported.Schema_User_GetRefreshToken(User));
        }

        private String _RefreshToken;
        public String RefreshToken
        {
            get => _RefreshToken;
        }
    }

    public class AuthenticationResponse
    {
        public AuthenticationResponse(Imports.Schema.AuthenticateResponse Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }
        }

        /// <summary> Gets First Error for AuthenticationResponse </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

    }

    public class DatastreamUpdate
    {
        public class Event
        {
            public Event(Imports.Schema.DatastreamEvent Obj)
            {
                this._Json = NativeString.StringFromUTF8AndDeallocate(Imported.Schema_DatastreamEvent_GetJson(Obj));
                this._Timestamp = Imported.Schema_DatastreamEvent_GetTimestamp(Obj);
            }

            private Int64 _Timestamp;
            public Int64 Timestamp
            {
                get => _Timestamp;
            }

            private String _Json;
            public String Json
            {
                get => _Json;
            }
        }

        public DatastreamUpdate(Imports.Schema.DatastreamUpdate Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            for (UInt32 i = 0; i < Imported.Schema_DatastreamUpdate_GetEventCount(Obj); i++)
            {
                _Events.Add(new Event(Imported.Schema_DatastreamUpdate_GetEventAt(Obj, i)));
            }
        }
        /// <summary> Gets First Error for DatastreamUpdate </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private List<Event> _Events = new List<Event>();
        public List<Event> Events
        {
            get => _Events;
        }
    }

    public class Transaction
    {
        public Transaction(Imports.Schema.TransactionResponse Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            this._Id = NativeString.StringFromUTF8(Imported.Schema_Transaction_GetId(Obj));
            this._SKU = NativeString.StringFromUTF8(Imported.Schema_Transaction_GetSKU(Obj));
            this._DisplayName = NativeString.StringFromUTF8(Imported.Schema_Transaction_GetDisplayName(Obj));
            this._UserId = NativeString.StringFromUTF8(Imported.Schema_Transaction_GetUserId(Obj));
            this._UserName = NativeString.StringFromUTF8(Imported.Schema_Transaction_GetUserName(Obj));
            this._Cost = Imported.Schema_Transaction_GetCost(Obj);
            this._Timestamp = NativeTimestamp.DateTimeFromMilliseconds(Imported.Schema_Transaction_GetTimestamp(Obj));
            this._Json = NativeString.StringFromUTF8AndDeallocate(Imported.Schema_Transaction_GetAdditionalJson(Obj));
        }

        /// <summary> Gets First Error for Transaction </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private String _Id;
        public String Id
        {
            get => _Id;
        }

        private String _SKU;
        public String SKU
        {
            get => _SKU;
        }

        private String _DisplayName;
        public String DisplayName
        {
            get => _DisplayName;
        }

        private String _UserId;
        public String UserId
        {
            get => _UserId;
        }

        private String _UserName;
        public String UserName
        {
            get => _UserName;
        }

        private Int32 _Cost;
        public Int32 Cost
        {
            get => _Cost;
        }

        private DateTime _Timestamp;
        public DateTime Timestamp
        {
            get => _Timestamp;
        }

        private String _Json;
        public String Json
        {
            get => _Json;
        }

    }

    public class OutstandingTransactions
    {
        public OutstandingTransactions(Imports.Schema.GetOutstandingTransactionsResponse Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            for (UInt32 i = 0; i < Imported.Schema_GetOutstandingTransactionsResponse_GetTransactionCount(Obj); i++)
            {
                _Transactions.Add(new Transaction(Imported.Schema_GetOutstandingTransactionsResponse_GetTransactionAt(Obj, i)));
            }

        }
        /// <summary> Gets First Error for GetOutstandingTransactionsResponse </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private List<Transaction> _Transactions = new List<Transaction>();
        public List<Transaction> Transactions
        {
            get => _Transactions;
        }
    }

    public class StateResponse
    {
        public StateResponse(Imports.Schema.StateResponse Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            this._Json = NativeString.StringFromUTF8AndDeallocate(Imported.Schema_StateResponse_GetJson(Obj));
        }
        /// <summary> Gets First Error for StateResponse </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private String _Json;
        public String Json
        {
            get => _Json;
        }

    }

    public class StateUpdate
    {
        public StateUpdate(Imports.Schema.StateUpdate Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            this._Target = NativeString.StringFromUTF8(Imported.Schema_StateUpdate_GetTarget(Obj));
            this._Json = NativeString.StringFromUTF8AndDeallocate(Imported.Schema_StateUpdate_GetJson(Obj));
        }

        /// <summary> Gets First Error for StateUpdate </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private String _Target;
        public String Target
        {
            get => _Target;
        }

        private String _Json;
        public String Json
        {
            get => _Json;
        }

    }

    public class ConfigResponse
    {
        public ConfigResponse(Imports.Schema.ConfigResponse Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            this._ConfigId = NativeString.StringFromUTF8(Imported.Schema_ConfigResponse_GetConfigID(Obj));
            this._Json = NativeString.StringFromUTF8AndDeallocate(Imported.Schema_ConfigResponse_GetJson(Obj));
        }

        /// <summary> Gets First Error for ConfigResponse </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private String _ConfigId;
        public String ConfigId
        {
            get => _ConfigId;
        }

        private String _Json;
        public String Json
        {
            get => _Json;
        }
    }

    public class ConfigUpdate
    {
        public ConfigUpdate(Imports.Schema.ConfigUpdate Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            this._ConfigId = NativeString.StringFromUTF8(Imported.Schema_ConfigUpdateResponse_GetConfigID(Obj));
            this._Json = NativeString.StringFromUTF8AndDeallocate(Imported.Schema_ConfigUpdateResponse_GetJson(Obj));
        }

        /// <summary> Gets First Error for ConfigUpdate </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private String _ConfigId;
        public String ConfigId
        {
            get => _ConfigId;
        }

        private String _Json;
        public String Json
        {
            get => _Json;
        }
    }

    public class GetPollResponse
    {
        public GetPollResponse(Imports.Schema.GetPollResponse Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            this._PollId = NativeString.StringFromUTF8(Imported.Schema_GetPollResponse_GetPollId(Obj));
            this._Prompt = NativeString.StringFromUTF8(Imported.Schema_GetPollResponse_GetPrompt(Obj));
            this._Mean = Imported.Schema_GetPollResponse_GetMean(Obj);
            this._Sum = Imported.Schema_GetPollResponse_GetSum(Obj);
            this._Count = Imported.Schema_GetPollResponse_GetCount(Obj);

            for (UInt32 i = 0; i < Imported.Schema_GetPollResponse_GetOptionCount(Obj); i++)
            {
                Options.Add(NativeString.StringFromUTF8(Imported.Schema_GetPollResponse_GetOptionAt(Obj, i)));
            }
            for (UInt32 i = 0; i < Imported.Schema_GetPollResponse_GetResultCount(Obj); i++)
            {
                Results.Add(Imported.Schema_GetPollResponse_GetResultAt(Obj, i));
            }
        }
        /// <summary> Gets First Error for GetPollResponse </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private String _PollId;
        public String PollId
        {
            get => _PollId;
        }

        private String _Prompt;
        public String Prompt
        {
            get => _Prompt;
        }

        private double _Mean;
        public double Mean
        {
            get => _Mean;
        }

        private double _Sum;
        public double Sum
        {
            get => _Sum;
        }

        private Int32 _Count;
        public Int32 Count
        {
            get => _Count;
        }

        private List<String> _Options = new List<String>();
        public List<String> Options
        {
            get => _Options;
        }

        private List<Int32> _Results = new List<Int32>();
        public List<Int32> Results
        {
            get => _Results;
        }

    }

    public class PollUpdateResponse
    {
        public PollUpdateResponse(Imports.Schema.PollUpdateResponse Obj)
        {
            NativeError Err = Imported.Schema_GetFirstError(Obj.Obj);
            if (Imported.Error_IsValid(Err))
            {
                this._FirstError = new Error(Err);
                return;
            }

            this._PollId = NativeString.StringFromUTF8(Imported.Schema_PollUpdateResponse_GetPollId(Obj));
            this._Prompt = NativeString.StringFromUTF8(Imported.Schema_PollUpdateResponse_GetPrompt(Obj));
            this._Mean = Imported.Schema_PollUpdateResponse_GetMean(Obj);
            this._Sum = Imported.Schema_PollUpdateResponse_GetSum(Obj);
            this._Count = Imported.Schema_PollUpdateResponse_GetCount(Obj);

            for (UInt32 i = 0; i < Imported.Schema_PollUpdateResponse_GetOptionCount(Obj); i++)
            {
                Options.Add(NativeString.StringFromUTF8(Imported.Schema_PollUpdateResponse_GetOptionAt(Obj, i)));
            }
            for (UInt32 i = 0; i < Imported.Schema_PollUpdateResponse_GetResultCount(Obj); i++)
            {
                Results.Add(Imported.Schema_PollUpdateResponse_GetResultAt(Obj, i));
            }
        }
        /// <summary> Gets First Error for PollUpdateResponse </summary>
        /// <returns> NULL if there is no error, otherwise error information </returns>
        private Error _FirstError = null;
        public Error GetFirstError()
        {
            return _FirstError;
        }

        private String _PollId;
        public String PollId
        {
            get => _PollId;
        }

        private String _Prompt;
        public String Prompt
        {
            get => _Prompt;
        }

        private double _Mean;
        public double Mean
        {
            get => _Mean;
        }

        private double _Sum;
        public double Sum
        {
            get => _Sum;
        }

        private Int32 _Count;
        public Int32 Count
        {
            get => _Count;
        }

        private List<String> _Options = new List<String>();
        public List<String> Options
        {
            get => _Options;
        }

        private List<Int32> _Results = new List<Int32>();
        public List<Int32> Results
        {
            get => _Results;
        }

    }
}