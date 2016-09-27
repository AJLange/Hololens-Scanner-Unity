var express  = require('express'),
    app      = express(),
    http     = require('http'),
    socketIo = require('socket.io');

// start webserver on port 8080
var server =  http.createServer(app);
var io     = socketIo.listen(server);
var table  = require('console.table');
var fs     = require('fs');
server.listen(8080);
// array of all lines drawn
var line_history = [];
var log          = console.log.bind(console);


var fs = require('fs');
var wstream = fs.createWriteStream('myOutput.txt');

// var readStream  = fs.CreateReadStream(__dirname,'/readMe.txt', 'utf8');
// var writeStream = fs.CreateWriteStream(__dirname, "/writeMe.txt");

// // Pipe from readable stream to writeable stream
// readStream.pipe(writeStream);


// add directory with our static files
app.use(express.static(__dirname + '/public'));
console.log("Server running on 127.0.0.1:8080");



// event-handler for new incoming connections
io.on('connection', function (socket) {


    socket.on('error', function(e) {
        console.log(e);
    });


    // Node.js 0.10+ emits finish when complete
    wstream.on('finish', function () {
    console.log('file has been written');
    });


   // first send the history to the new client
   for (var i in line_history) {
      socket.emit('draw_line', { line: line_history[i] } );
   }

   // add handler for message type "draw_line".
   socket.on('draw_line', function (data) {
      // add received line to history
      line_history.push(data.line);
      // send line to all clients
      io.emit('draw_line', { line: data.line });
   });



    socket.on('ConvertedImgToBytes', function (bytes) {
        // socket.emit('ConvertBytesToImg', bytes);
        socket.emit('image', bytes);
        //   console.table(bytes);
    });


    // http://stackoverflow.com/questions/6182315/how-to-do-base64-encoding-in-node-js
    socket.on('SendImageHTML', function (html) {
        console.log("SendImageHTML called: ");
        log(html);
        // RETURNS:
        // SendImageHTML called:
        // {} 
        socket.broadcast.emit('receiveHTML', html);

    });


    socket.on('divimg', (payload) => {
         socket.broadcast.emit('divimg', payload);
    });

   socket.on('imgBinary', (payload) => {
         socket.broadcast.emit('imgBinary', payload);
    });

    
    socket.on('defImgBinary', function (bytes) {
        log("defImgBinary called: ");
        // log(bytes);
        // RETURNS:
        // defImgBinary called:
        // '53721': 220,
        // '53722': 219,
        // '53723': 255,
        // '53724': 229,
        socket.broadcast.emit('ConvertBytesToImg', bytes);
    });


});