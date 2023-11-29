"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("button").disabled = true;

connection.start().then(function () {
    document.getElementById("button").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
