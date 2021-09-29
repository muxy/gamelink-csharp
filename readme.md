# Muxy GameLink C# Library

## Requirements

Before you get started you will want to register your extension in the [Muxy Dashboard](dev.muxy.io)

## Getting Started

Simply copy the files from this repo into your project.

You will need your own JSON library and Websocket library to properly use GameLink.

## Documentation

Documentation for the GameLink C# library beyond example usages is currently non-existent, but you may refer to the [C++ documentation](dev.muxy.io/docs/api) for the time being as the two libraries are very close in usage. C# documentation is currently being worked on and will be coming soon.

## Websocket Transport
The class `MuxyGameLink.WebsocketTransport` offers an implementation of a websocket transport based off System.Net.WebSockets.
To use the provided transport, create an instance of `WebsocketTransport` after creating an SDK instance, and then call
`ConnectAndRunInStage`:

```C#
void StartGameLink(string pin)
{
	var gamelink = new SDK("<Twitch Client ID>");
	var transport = new WebsocketTransport();

	transport.ConnectAndRunInStage(gamelink, Stage.Sandbox);

	gamelink.AuthenticateWithPIN(pin, (AuthenticationResponse Resp) =>
	{
		Error e = Resp.GetFirstError();
		if (e != null)
		{
			Debug.Log("Got response from auth: " + e.ToString());
		}
		else
		{
			Debug.Log("Got response from auth: OK");
		}
	});
}
```
