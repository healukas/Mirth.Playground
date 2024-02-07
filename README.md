# Mirth.Playground
Welcome to the Mirth.Playground. I am using this repository to play with and learn about [Mirth Connect](https://github.com/nextgenhealthcare/connect/). 
I am also documenting my learnings as good as I can. 

## What's in the box?
This Playground contains some toys to play and experiment with [Mirth Connect](https://github.com/nextgenhealthcare/connect/). 
Included are:
- `MirthSocketClient`: a program that writes HL7 Messages to a Socket (Using MLLPv2)
- `MirthSocketServer`: a program that provides a socket and reads from it (Using MLLPv2)
- `Stack`: a docker stack file running Mirth Connect, a database and a debug rest endpoint (for `HTTPSender` testing). It also includes some channel definitions to import into Mirth Connect.
- `Documentation`: A collection of Markdown files (and some html files) where I am trying to best describe what I did and what I learned.

### MirthSocketClient
The `MirthSocketClient` currently always sends the same message with a counter in MSH-3.
The variables at the top of `MirthSocketClient > Program.cs` can be used to change the behavior. Currently no command line parameters are supported since 
the program is mostly ran from the IDE. 

The configurable variables are:
- `HostName`: the hostname to connect to. This can be an IP Address or a hostname. It will be resolved using DNS.
- `Port`: the port to connect to
- `Verbose`: If `true` the sent messages and received messages are written to the Console.
- `AmountOfMessages`: The number of messages to be sent. 
- `MsBetweenMessages`: The time between messages in milliseconds.

To start the program, use your IDE or `dotnet run .` in the directory `Mirth.Playground > MirthSocketClient`.

### MirthSocketServer
The `MirthSocketServer` creates a socket and receives data on that socket. 

It can be configured to listen to a specific IP address at a specific port.
The values can be set in the variables 
- `HostIp`
- `Port`

When receiving any payload, the `MirthSocketServer` sends the 4-byte ACK as specified in `Documentation/HL7MLLP/transport_mllp_2019.html`.
The `MirthSocketServer` runs in an infinite loop until it is terminated by the user (or until an exception occurs for). 

To run it, start it from the IDE or use `dotnet run .` in the directory `Mirth.Playground > MirthSocketServer`.

### Stack
The current version of the stack includes:
- Mirth Connect
- Database
- A debug rest endpoint that is used for testing the `HTTPSender` in Mirth.

Please keep in mind that the forwarded ports are exactly the ones required for the `MirthSocketClient`. If you intend to change 
the port in the client, please also open it here, so the port is reachable.

### JavaLibs
A minimal working transformation of the `PID` segment to a basic FHIR Patient resource written in Java.
This is meant to be used as a custom transformer/preprocessing script. However, even when building it as a fatJar and if it is possible to execute that jar, it currently still fails when using it in Mirth. 
I have to figure out how to build it such that mirth finds all the dependencies. 

### Documentation
The `Documentation` directory is set up so that it can be viewed with [docsify](https://docsify.js.org/#/). Alternatively, you can just look at the files in the 
markdown reader of your choice. 

Once you have [docsify](https://docsify.js.org/#/) installed, go to the root directory of this project and run 
```shell 
docsify serve Documentation -p 3000
```
This will start a simple web server rendering the markdown files for you to watch at `localhost:3000`.