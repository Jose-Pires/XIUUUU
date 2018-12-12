# TrustAgent Changes

* Fixed verification of entity already connected error upon message received from said client
* ClientHandler message decoder was skiping 4 bytes leading to missing bytes on the string
* Removed unecessary coments
* Comand from client "disconnect" is now recognized by the server
* Comand from client "request_key_negotiation" is now recognized by the server (everything was implemented except the comand recognizer for this comand)

TrustAgent Released as 1.0.0