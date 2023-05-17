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

/* GET controller page. */
router.get('/1', function (req, res, next) {
  res.render('controller', { player: '1' });
});

router.get('/2', function (req, res, next) {
  res.render('controller', { player: '2' });
});

router.get('/3', function (req, res, next) {
  res.render('controller', { player: '3' });
});

router.get('/4', function (req, res, next) {
  res.render('controller', { player: '4' });
});

router.get('/game/:character/run-left', function (req, res) {
  pusher.trigger('my-channel', 'run-left', {
    "playerNumber": req.params.character,
  });
  res.send();
});

router.get('/game/:character/run-right', function (req, res) {
  pusher.trigger('my-channel', 'run-right', {
    "playerNumber": req.params.character,
  });
  res.send();
});

router.get('/game/:character/attack', function (req, res) {
  pusher.trigger('my-channel', 'attack', {
    "playerNumber": req.params.character,
  });
  res.send();
});

router.get('/game/:character/idle', function (req, res) {
  pusher.trigger('my-channel', 'idle', {
    "playerNumber": req.params.character,
  });
  res.send();
});

router.post('/game/:character/chat', function (req, res) {
  pusher.trigger('my-channel', 'chat', {
    "playerNumber": req.params.character,
    "chatMessage": req.body.chatMessage
  });
  res.send();
});

module.exports = router;
