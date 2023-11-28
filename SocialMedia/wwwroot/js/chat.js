﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

var user = document.getElementById("userInput").value;
var message = document.getElementById("messageInput").value;
connection.invoke("SendMessage", user, message).catch(function (err) {
    return console.error(err.toString());
});