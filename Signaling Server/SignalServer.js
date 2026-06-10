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
	const playerCount = clients.size;
    for (const client of clients) {
      
      if (client !== ws && client.readyState === WebSocket.OPEN) { // Don't send message back to sender
        client.send(JSON.stringify(data));
      }
	  if (client.readyState === WebSocket.OPEN && data.type === "join"){ // Send player count when new peer joined
		client.send(JSON.stringify({
		  room: room,
		  type: "playerCountUpdate",
		  count: playerCount
		}));
	  }
	  if (client.readyState === WebSocket.OPEN && playerCount === 2){
		client.send(JSON.stringify({
		  room: room,
		  type: "matchReady",
		}));
		console.log(roomName+": is ready");
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
