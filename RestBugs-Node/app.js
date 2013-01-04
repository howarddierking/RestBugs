/*Setup*/
var express = require('express');

var databaseUrl = "restbugs";
var collections = ["bugs"];
var mongo = require('mongojs');
var db = mongo.connect(databaseUrl, collections);

var app = express.createServer();
app.listen(9200);

app.configure(function(){
	app.register('.html', require('ejs'));
	app.set('view options', {
		layout: false
	});
	app.use(express.bodyParser());	//where is app.use defined and what does it do??
});

/*Helper functions*/

function addToHistory(doc, comments, stateChanges){
	if(doc.history === undefined)
		doc.history = [];

	doc.history.push({
		addedOn: new Date(),
		comments: comments,
		changes: stateChanges
	});
};

function updateStatus(doc, status, comments){
	doc.status = status;
	addToHistory(doc, comments, {status : doc.status});
};

function activate(doc, comments){
	updateStatus(doc, 'Working', comments);
};

function resolve(doc, comments){
	updateStatus(doc, 'QA', comments);
};

function close(doc, comments){
	updateStatus(doc, 'Done', comments);
};

function toBacklog(doc, comments){
	updateStatus(doc, 'Backlog', comments);
};

function newbug(title, description){
	var bug = {		
		title: title,
		description: description
	};
	toBacklog(bug, "bug created");
	return bug;
};

/*API Surface*/

app.get('/bugs', function(req, res){
	res.render('bugs-all.html', { title: "Bugs API root"});
});

app.get('/bugs/backlog', function(req, res){
	db.bugs.find({status: 'Backlog'}, function(err, docs) {
		res.render('bugs-all.html', { 
			title: "Backlog", 
			model: docs 
		});	
	});
});

app.post('/bugs/backlog', function(req, res){
	//todo: consider replacing with upsert-style call
	if(req.body.id===undefined) {
		db.bugs.save(
			newbug(req.body.title, req.body.description), 
			function(err, savedDoc) {
				db.bugs.find( {status: 'Backlog'}, function(err, docs) {
					res.render('bugs-all.html', {
						title: 'Backlog',
						model: docs
					});
				});
		});
	} else {
		db.bugs.findOne( {_id: mongo.ObjectId(req.body.id) }, function(err, doc) {
			//todo: return 404 if doc is undefined

			toBacklog(doc, req.body.comments);

			db.bugs.update( {_id: mongo.ObjectId(req.body.id) }, doc, function(err, updatedDoc){
				db.bugs.find({status:'Backlog'}, function(err, docs){
					res.render('bugs-all.html', { 
						title: "Backlog", 
						model: docs 
					});	
				});
			});
		});
	}
});

app.get('/bugs/working', function(req, res){
	db.bugs.find({status:'Working'}, function(err, docs){
		res.render('bugs-all.html', { 
			title: "Working", 
			model: docs 
		});	
	});
});

app.post('/bugs/working', function(req, res){
	db.bugs.findOne( {_id: mongo.ObjectId(req.body.id) }, function(err, doc) {
		//todo: return 404 if doc is undefined

		activate(doc, req.body.comments);

		db.bugs.update( {_id: mongo.ObjectId(req.body.id) }, doc, function(err, updatedDoc){
			db.bugs.find({status:'Working'}, function(err, docs){
				res.render('bugs-all.html', { 
					title: "Working", 
					model: docs 
				});	
			});
		});
	});
});

app.get('/bugs/qa', function(req, res){
	db.bugs.find({status:'QA'}, function(err, docs){
		res.render('bugs-all.html', { 
			title: "QA", 
			model: docs 
		});	
	});
});

app.post('/bugs/qa', function(req, res){
	db.bugs.findOne( {_id: mongo.ObjectId(req.body.id) }, function(err, doc) {
		//todo: return 404 if doc is undefined

		resolve(doc, req.body.comments);

		db.bugs.update( {_id: mongo.ObjectId(req.body.id) }, doc, function(err, updatedDoc){
			db.bugs.find({status:'QA'}, function(err, docs){
				res.render('bugs-all.html', { 
					title: "QA", 
					model: docs 
				});	
			});
		});
	});
});

app.get('/bugs/done', function(req, res){
	db.bugs.find({status:'Done'}, function(err, docs){
		res.render('bugs-all.html', { 
			title: "Done", 
			model: docs 
		});	
	});
});

app.post('/bugs/done', function(req, res){
	db.bugs.findOne( {_id: mongo.ObjectId(req.body.id) }, function(err, doc) {
		//todo: return 404 if doc is undefined

		close(doc, req.body.comments);

		db.bugs.update( {_id: mongo.ObjectId(req.body.id) }, doc, function(err, updatedDoc){
			db.bugs.find({status:'Done'}, function(err, docs){
				res.render('bugs-all.html', { 
					title: "Done", 
					model: docs 
				});	
			});
		});
	});
});

/*first, let's remove any initial values in the database*/
db.bugs.remove({});
