# Drastic.Tempest

[![NuGet version (Drastic.Tempest)](https://img.shields.io/nuget/v/Drastic.Tempest.svg?style=flat-square)](https://www.nuget.org/packages/Drastic.Tempest/)

Drastic.Tempest is a hard fork of [Tempest](https://github.com/ermau/Tempest) by [Eric Maupin](https://github.com/ermau). Drastic.Tempest updates the base targets to `net6.0` and removes the older device specific implementations, which are not needed anymore. You should be able to use this on any platform `net6.0` targets. It also removes the use of `BinaryFormatter`, which is not supported for `net6.0` and up platforms. It also removes the `SAFE` implementation.

Otherwise, it's the same as the existing Tempest library and can be used as such.

# Tempest

From the original documentation:

> Tempest is a simple library for sending and receiving messages across
any number of transports and dealing with them in a uniform manner. For
example, you could receive the same message from a HTTP POST and from a
TCP socket and from your perspective there would be no difference in
handling it.

**Features:**

 - Simple protocol definition
 - Encryption
 - Message signing
 - Multiple message transports (raw tcp, http, etc.) feeding to the same
   message handlers
 - Supports multiple Tempest protocols over a single connection

## Getting Started

View the `samples` directory for example projects and documentation on how to use Drastic.Tempest.

### Transports
There are currently two available transports:

 - TCP
   - Supports reliable messaging
   - Supports encryption and signing
   - `NetworkClientConnection` for client connections
   - `NetworkConnectionProvider` for connection listeners
 - UDP
   - _Experimental_
   - Supports reliable and unreliable messaging
   - Supports encryption and signing
   - `UdpClientConnection` for client connections
   - `UdpConnectionProvider` for connection listeners

