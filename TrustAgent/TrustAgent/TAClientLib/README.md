# Trust Agent Client Library

A simple library that contains all the methods to connect to the Trust Agent.
All the complex operations are handled by this library so there's no need to perform nothing complicated on the program.

## Requirements

* .NET Core 2
* Network Connection (to communicate with the server

## Usage

Each example will use code from previous examples.

### Initialization

The initialization of the class is really simple as you need the entity name and key, the key must be an hexadecimal string and the key must be an 256bit key.

To initialize the class follow this example

    using TAClientLib
    
    class Program {
        TAClient taClient;
        static void Main(string[] args) {
            //Assuming that the args[0] contains the entity name and args[1] contains the 256 hexadecimal key
            taClient = new TAClient(args[0], args[1]);
        }
    }
And we have the TAClient initialized.

### Safely initialize the the TAClient

TAClient as a validation on initialization and it will produce exceptions on initialization. Those exceptions are `InvalidKeyException` witch contains a message inside with the error, those messages can be:

* Non hexadecimal key
* Key is not 256bit long

So this is an example on how to safely initialize the TAClient

    using TAClientLib
    
    class Program {
        TAClient taClient;
        static void Main(string[] args) {
            //Assuming that the args[0] contains the entity name and args[1] contains the 256 hexadecimal key
            try {
                taClient = new TAClient(args[0], args[1]);
            } catch (InvalidKeyException ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }

### Handle all Events

There's only a handful of events and only two of them have parameters here's how to handle those events

    using TAClientLib
    
    class Program {
        TAClient taClient;
        static void Main(string[] args) {
            // Assuming that the class has been already initialized
            taClient.Connected += TaClient_Connected;                   // The connection to the client was stablished
            taClient.Kicked += TaClient_Kicked;                         // The server kicked the client
            taClient.Disconnected += TaClient_Disconnected;             // The connection to the server was closed by the server
            taClient.EntityListReceived += TaClient_EntityListReceived; // The list with the online entities was received
            taClient.KeyReceived += TaClient_KeyReceived;               // The key was successfully negotiated
        }

        // Remember that C# methods should have the first letter as uppercase
        // Not mandatory but recommended
        static void TaClient_Connected() {
            Console.WriteLine("Client Connected");
        }
        
        static void TaClient_Kicked() {
            Console.WriteLine("Client Kicked");
        }

        static void TaClient_Disconnected() {
            Console.WriteLine("Server disconnected");
        }

        static void TaClient_EntityListReceived(List<string, string> e) {
             // The connected entities are received as a list of tuples
             // The tuples contain the entity name and ip
            foreach ((string, string) entity in e)
                Console.WriteLine(string.Format("Entity: {0} IP: {1}", entity.Item1, entity.Item2));
        }
        
        static void TaClient_KeyReceived(byte[] key, IPAddress remoteIP, int remotePORT) {
            // Now we have the key the IP and Port on witch the entity is waiting for us to connect to
        }
    }

Every single event needed is now handled.

### Connecting to the Trust Agent Server

Connecting to the Trust Agent Server is fairly easy, but you must handle the proper exceptions and there are quite a few.

In this example is shown how to connect to the server handling all possible exceptions

    using TAClientLib
    
    class Program {
        TAClient taClient;
        static void Main(string[] args) {
            // Assuming that the class has been already initialized
            try {
                // The port must be an int
                taClient.AttemptConnect("server ip", 12345);
            } catch (ClientRejectedException cre) {
                Console.WriteLine(cre.Message); // Prints the specific reason for this exception
            } catch (ConnectionFailedException) {
                // We do not declare a variable because there's never a message in this exception
                Console.WriteLine("Connection failed");
            } catch (HMACFailedException hmacfe) {
                // This Exception not only provides a standard message saying that the HMAC is not valid
                // but also contains the original HMAC and the Computed as HEX strings
                Console.WriteLine("HMAC Validation Failed!");
                Console.WriteLine(string.Format("Original HMAC: {0}\nComputed HMAC: {1}", hmacfe.ReceivedHMAC, hmacfe.ComputedHMAC));
            } catch (Exception ex) {
                // As there are other exception that aren't specific they can must be handled
                // by the generic "Exception" type
                Console.WriteLine(ex.Message);
            }
        }
    }

### Requesting the entities connected to the Trust Agent

We must know witch entities are online in order to negotiate a key with them this is how it can be achieved.

using TAClientLib
    
    class Program {
        TAClient taClient;
        static void Main(string[] args) {
            // Assuming that the class has been already initialized
            // The connection has been established
            // All the events are being handled
           
            try {
                taClient.RequestConnectedEntities();	
            }//....
            // I recomend to use the same catches explained above
            // as the same errors can occurr
        }
    }

The event `EntityListReceived` will be fired when the server sends the response. 

View the section **Handle all Events** to see how to handle the events.

### Initializing a Key Negotiation

So the objective is to negotiate a symmetric cipher key using a Trust Agent, we already know how to initialize the class `TAClient`, connect to the server and request the online entities, now let's see how to negotiate the key.

It's very simple for both the received or the sender.

For the sender

using TAClientLib
    
    class Program {
        TAClient taClient;
        static void Main(string[] args) {
            // Assuming that the class has been already initialized
            // The connection has been established
            // All the events are being handled
            // We already have the name of an online entity to witch we will negotiate the key
            
            try {
                // The second parameter contains the port on witch our "messenger" is listening, 
                // this is now outside the scope of this library
                taClient.RequestKey("entity to connect to", 12345);	
            }//....
            // I recomend to use the same catches explained above
            // as the same errors can occurr
        }
    }

For the receiver it's even easier, just handle the event.
View the section **Handle all Events** to see how to handle the events.

## Known bugs

There are currently no known bugs, but the library wasn't fully tested.
The entity retrieval and the key negotiation are working properly as are the events.
All the exceptions have been tested and none was thrown unintentionally.

Any bugs found please report them in the issues section of this repository.

## Testing

* This library was tested using the fully functional Trust Agent Server
* This library was implemented to run a small program that did not handle all events nor exceptions, only handled the `Connected`, `Kicked`, `Entity`, `EntityListReceived` and `KeyReceived` events. The methods `AttemptConnect`, `RequestConnectedEntities`, `RequestKey` were tested
* The Trust Agent Server was running on a macOS Mojave (10.14.1)
* The library was tested on macOS Mojave (10.14.1) and Windows 10 (b1809)
