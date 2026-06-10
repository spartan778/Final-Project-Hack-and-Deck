//Refenced and modified offical Godot Signaling-RTC demo at https://github.com/godotengine/godot-demo-projects under MIT license

const WebSocket = require("ws"); // Using WebSocket package
const PORT = 8080;
const wss = new WebSocket.Server({ port: PORT });
const rooms = new Map();

wss.on("connection", (ws) => {
	console.log("Client connected");
  let roomName = null; //init roomName at start of connection
  // When a message is received from this client
  ws.on("message", (msg) => {
    let data;
	console.log("message received");
    // Try to parse incoming message as JSON
    try {
      data = JSON.parse(msg);
    } catch {
      // Ignore invalid JSON messages
      console.log("Invalid JSON received");
      return;
    }

    // Extract room name from message
    const room = data.room;
    if (!room) return; // Ignore if no room specified

    // If this is the first message from the client,
    // assign them to a room
    if (!roomName) {
      roomName = room;

      // Create room if it doesn't exist
      if (!rooms.has(room)) {
        rooms.set(room, new Set());
      }

      // Add this client to the room
      rooms.get(room).add(ws);
      console.log("Client joined room:", room);
	  console.log("Room size:", rooms.get(room).size);
    }

    // Relay the message to all OTHER clients (peers) in the same room
    const clients = rooms.get(room);
    for (const client of clients) {
      // Don't send message back to sender
      if (client !== ws && client.readyState === WebSocket.OPEN) {
        client.send(JSON.stringify(data));
      }
    }
  });

  // When the client disconnects
  ws.on("close", () => {
    if (roomName && rooms.has(roomName)) {
      // Remove client from the room
      rooms.get(roomName).delete(ws);

      // If room is empty, delete it
      if (rooms.get(roomName).size === 0) {
        rooms.delete(roomName);
      }

      console.log("Client left room:", roomName);
    }
  });
});

// Startup message
console.log("Signaling server running on Port: " + PORT);
