"use strict";
const chatContainer = document.querySelector(".chat-window-text-container");
const input = document.querySelector(".chat-input");
const sendBtn = document.querySelector(".send-btn");
const ws = new WebSocket("ws://localhost:7071/");
const randomId = Math.trunc(Math.random() * 10) + 1;
const userData = { id: randomId };
ws.addEventListener("open", () => {
    ws.send(JSON.stringify(userData));
});
console.log(randomId);
sendBtn.addEventListener('click', () => {
    ws.send(input.value);
    ws.addEventListener("message", (e) => {
        console.log(e);
    });
});
