# ChatApplication

ChatApplication uses SignalR and Sqlite to make real time communication. SignalR is an RPC protocol created by Microsoft to allow real time communication.
It can be used for chats, games, messages etc. It can be featured as an RPC protocol, but the lib does not provide an way to share any data that user didnt
ask for it, and can only be used to share messages. But at the end of the day, you can always encode an byte array in a kind of UTF-8 encoding and use it 
for share big batches of data, but we have more specialized libs to do it.

## How to use it.
Enter in the project folder:
``` bash
cd ChatApplication
```

First, generate the appropriatte Docker image from application. If you have the Make tool installed on your machine, you can simple run ```make``` to generate 
an image called 'chatapp':

``` bash
make
```
or using ``` docker build``` command:
``` bash
docker build -t chatapp .
```

The application image will be generated, then you can use it along with Docker compose or standalone with the ```docker run``` command:

``` bash
docker run --rm --name chatapp_container -p 8080:8080 -d chatapp
```
or using docker compose:

Move the docker compose example file and run it:
``` bash
mv docker-compose.example.yml docker-compose.yml
docker compose up -d 
```

Now you can acess it from browser, through the host:8080 or whatever the door it will be running.


I have hosted it in my personal home lab, so you can acess [it](https://chatapp.darkpass.com.br) on the browser.

The code is free to make any customization. Enjoy it! The messages will be saved!
