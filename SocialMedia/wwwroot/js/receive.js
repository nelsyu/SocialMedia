"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (message, sendTime) {
    var li = document.createElement("li");
    li.classList.add("list-group-item", "text-light");
    document.getElementById("messagesList").appendChild(li);
    li.innerHTML = `${message}<br>${sendTime}`;
});

