 'use strict';

 var nconf = require ('nconf'); 

 nconf
 .file({file: './localconfig.json'})
 .env();

 var connectionString = nconf.get("IOTPrimaryKey");
 console.log(connectionString);
 var clientFromConnectionString = require('azure-iot-device-amqp').clientFromConnectionString;
 var Message = require('azure-iot-device').Message;



 var client = clientFromConnectionString(connectionString);

 function printResultFor(op) {
   return function printResult(err, res) {
     if (err) console.log(op + ' error: ' + err.toString());
     if (res) console.log(op + ' status: ' + res.constructor.name);
   };
 }

 var connectCallback = function (err) {
   if (err) {
     console.log('Could not connect: ' + err);
   } else {
     console.log('Client connected');

     // Create a message and send it to the IoT Hub every second
     setInterval(function(){
         var temp = 10 + (Math.random() * 4);
         var light = 10 + (Math.random() * 4);
         var data = JSON.stringify({ deviceId: 'myFirstNodeDevice', Tempurature: temp, Light: light });
         var message = new Message(data);
         console.log("Sending message: " + message.getData());
         client.sendEvent(message, printResultFor('send'));
     }, 1000);
   }
 };

  client.open(connectCallback);