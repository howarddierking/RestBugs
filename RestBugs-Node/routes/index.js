// todo: add all of the necessary module imports
var bugs = require('../lib/bugsdata'),
	_ = require('underscore'),
	b = require('../lib/bugs'),
	util = require('util'),
	assert = require('assert');

// render helper
function renderView(res, title, model) {
	debugger;

	res.set('Cache-Control', 'no-cache, no-store');

	var viewModel = {
		title: title,
		model: model,
		toAbsoluteUrl: function(rootRelativeUrl){
			return 'http://' + res.req.headers.host + rootRelativeUrl;
		}
	}
	res.format({
		'text/html': function() {
			res.render('bugs-all-html', viewModel);				
		},

		'application/json': function() {
			res.render('bugs-all-json', viewModel);				
		}
	});
};

var listView = function(res, title){
	return function(err, docs){
		renderView(res, title, docs);
	};
};

// routes

exports.index = function(req, res){
	renderView(res, 'Index');
};

exports.backlog = function(req, res){
	bugs.lists.backlog(listView(res, 'Backlog'));
};

exports.backlog_post = function(req, res){
debugger;
	var bugID = req.body.id,
		title = req.body.title,
		description = req.body.description,
		comments = req.body.comments;

	if(bugID){
		bugs.getBug(bugID, function(err, data){
			if(err)
				return res.send(500, err);
			data.backlog(comments);
			bugs.saveBug(data, function(err, data){
				if(err)
					return res.send(500, err);
				bugs.lists.backlog(listView(res, 'Backlog'));
			});
		});
	} else {
		var bug = b.create(title, description);
		bugs.saveBug(bug, function(err, data){
			if(err)
				return res.send(500, err);
			bugs.lists.backlog(listView(res, 'Backlog'));
		});
	}
};

exports.working = function(req, res){
	bugs.lists.working(listView(res, 'Working'));
};

exports.working_post = function(req, res){
	var bugID = req.body.id,
		comments = req.body.comments;

	assert(bugID, 'Bug ID is required.')

	bugs.getBug(bugID, function(err, data){
		if(err)
			return res.send(500, err);

debugger;

		data.working(comments);
		bugs.saveBug(data, function(err, data){
			if(err)
				return res.send(500, err);
			bugs.lists.working(listView(res, 'Working'));
		});
	});
};

exports.qa = function(req, res){
	bugs.lists.qa(listView(res, 'QA'));
};

exports.qa_post = function(req, res){
	var bugID = req.body.id,
		comments = req.body.comments;

	assert(bugID, 'Bug ID is required.')

	bugs.getBug(bugID, function(err, data){
		if(err)
			return res.send(500, err);
		data.qa(comments);
		bugs.saveBug(data, function(err, data){
			if(err)
				return res.send(500, err);
			bugs.lists.qa(listView(res, 'QA'));
		});
	});
};

exports.done = function(req, res){
	bugs.lists.done(listView(res, 'Done'));
};

exports.done_post = function(req, res){
	var bugID = req.body.id,
		comments = req.body.comments;

	assert(bugID, 'Bug ID is required.')

	bugs.getBug(bugID, function(err, data){
		if(err)
			return res.send(500, err);
		data.done(comments);
		bugs.saveBug(data, function(err, data){
			if(err)
				return res.send(500, err);
			bugs.lists.done(listView(res, 'Done'));
		});
	});
};