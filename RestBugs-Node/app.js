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

/*function Bug(config) {
	var self = this;		// this seems to be necessary so that updateStatus works - WHY???
	
	// I *think* this is the right way to make privately scoped functions for the Bug object
	function addToHistory(comments, stateChanges){
		self.history.push({
			addedOn: new Date(),
			comments: comments,
			changes: stateChanges
		});
	};

	function updateStatus(status, comments){
		self.status = status;
		addToHistory(comments, {status : self.status});
	};

	// I *think* this is the right way to make privelleged functions for the Bug object
	this.activate = function(comments){
		updateStatus('Working', comments);
	};

	this.resolve = function(comments){
		updateStatus('QA', comments);
	};

	this.close = function(comments){
		updateStatus('Done', comments);
	};

	this.toBacklog = function(comments){
		updateStatus('Backlog', comments);
	};

	//todo: null checking on config
	//title, description, jsonObj
	if(config.jsonObj === undefined){
		//create a new instance
		this.title = config.title;
		this.description = config.description;
		this.assignedTo = '';
		this.status = '';
		this.history = [];

		this.toBacklog("created");
	} else{
		this.title = config.jsonObj.title;
		this.description = config.jsonObj.description;
		this.assignedTo = config.jsonObj.assignedTo;
		this.status = config.jsonObj.status;
		this.history = config.jsonObj.history===undefined ? [] : config.jsonObj.history;
	}
};*/

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






app.get('/bugs', function(req, res){
	res.render('bugs-all.html', { title: "Bugs API root"});
});

app.get('/bugs/backlog', function(req, res){
	db.bugs.find( {status: 'Backlog'}, function(err, docs) {
		res.render('bugs-all.html', { 
			title: "Backlog", 
			model: { bugs : docs }});	
	});
});

app.post('/bugs/backlog', function(req, res){
	//this could be either a create or an update
	//TODO: replace with upsert-style call
	if(req.body.id===undefined) {
		console.log('add a new bug');
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
	}
});

app.get('/bugs/working', function(req, res){
	db.bugs.find({status:'Working'}, function(err, docs){
		res.render('bugs-all.html', { 
			title: "Working", 
			model: { bugs : docs }});	
	});
});

app.post('/bugs/working', function(req, res){
	db.bugs.findOne( {_id: mongo.ObjectId(req.body.id) }, function(err, doc) {
		//todo: return 404 if doc is undefined

		//update the doc
		updateStatus(doc, 'Working', req.body.comments);

		db.bugs.update( {_id: mongo.ObjectId(req.body.id) }, doc, function(err, updatedDoc){
			db.bugs.find({status:'Working'}, function(err, docs){
				res.render('bugs-all.html', { 
				title: "Working", 
				model: { bugs : docs }});	
			});
		});
	});
});

app.get('/bugs/qa', function(req, res){
	db.bugs.find({status:'QA'}, function(err, docs){
		res.render('bugs-all.html', { 
			title: "QA", 
			model: { bugs : docs }});	
	});
});

app.post('/bugs/qa', function(req, res){
	db.bugs.find({_id:req.body.id}, function(err, doc){
		doc.resolve(req.body.comments);

		db.bugs.update({_id:req.body.id}, doc, function(err, updatedDoc){
			db.bugs.find({status:'QA'}, function(err, docs){
				res.render('bugs-all.html', { 
				title: "QA", 
				model: { bugs : docs }});	
			});
		});
	});
});

app.get('/bugs/done', function(req, res){
	db.bugs.find({status:'Done'}, function(err, docs){
		res.render('bugs-all.html', { 
			title: "Done", 
			model: { bugs : docs }});	
	});
});

app.post('/bugs/done', function(req, res){
	db.bugs.find({_id:req.body.id}, function(err, doc){
		doc.close(req.body.comments);

		db.bugs.update({_id:req.body.id}, doc, function(err, updatedDoc){
			db.bugs.find({status:'Done'}, function(err, docs){
				res.render('bugs-all.html', { 
				title: "Done", 
				model: { bugs : docs }});	
			});
		});
	});
});

// first, let's remove any initial values in the database
db.bugs.remove({});