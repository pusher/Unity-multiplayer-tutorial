var express = require('express');
var router = express.Router();

var Pusher = require('pusher');

var pusher = new Pusher({
  appId: 'appId',
  key: 'appKey',
  secret: 'appSecret',
  cluster: 'appCluster',
  useTLS: true
});

/* GET users listing. */
router.get('/', function (req, res, next) {
  res.send('respond with a resource');
});

router.post("/auth", (req, res) => {
  const socketId = req.body.socket_id;

  // Replace this with code to retrieve the actual user id and info
  const user = {
    id: req.body.player,
    user_info: {
      name: "John Smith",
    },

  };
  const authResponse = pusher.authenticateUser(socketId, user);
  res.send(authResponse);
});

router.post("/game-auth", (req, res) => {
  const socketId = req.body.socket_id;

  const user = {
    id: "game",
    user_info: {
      name: "John Smith",
    },
    watchlist: ['1', '2', '3', '4']
  };
  const authResponse = pusher.authenticateUser(socketId, user);
  res.send(authResponse);
});

router.get("/terminate/:player", (req, res) => {
  const authResponse = pusher.terminateUserConnections(req.params.player);
  res.send("Player " + req.params.player + "was disconnected");
});

module.exports = router;