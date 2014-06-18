/*Setup*/
var express = require('express');
var util = require('util');
var http = require('http');
var routes = require('./routes');

var app = express();

app.set('port', process.env.PORT || 3000);

app.set('views', __dirname + '/views');
app.set('view engine', 'ejs');	
app.set('view options', {
	layout: false
});
app.use(express.bodyParser());
app.use(app.router);

// routes 
app.get('/bugs', routes.index);
app.get('/bugs/backlog', routes.backlog);
app.post('/bugs/backlog', routes.backlog_post);
app.get('/bugs/working', routes.working);
app.post('/bugs/working', routes.working_post);

app.get('/bugs/qa', routes.qa);
app.post('/bugs/qa', routes.qa_post);

app.get('/bugs/done', routes.done);
app.post('/bugs/done', routes.done_post);

// start the server
http.createServer(app).listen(app.get('port'), function(){
  console.log('Express server listening on port ' + app.get('port'));
});
