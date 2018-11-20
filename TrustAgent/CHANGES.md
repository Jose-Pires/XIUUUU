* Removed PacketType.ServerResponse
* Added ServerOperations.ResponseSuccessEntities
* Added ServerOperations.InvalidHMAC
* Added EntityNoLongerAvailable
* Removed unecessary variable declaration on ClientHandler catch (Line 76)
* Upon received message, a verification is made to check if the packet is valid, if an error ocurrs the error is sent to the client otherwise we perform the next step, process the message to understand the operation requested
* Verifies if the operation requested by the client is valid
* Sends the Connected Entities List to the client
* Added CHANGES.md to keep track of the changes made to the TrustAgent project
