// todo: add all of the necessary module imports
var bugs = require('../lib/bugsdata'),
	_ = require('underscore'),
	b = require('../lib/bugs'),
	util = require('util'),
	assert = require('assert'),
	headerLinking = require('web-linking').httpheader;

var _toAbsoluteUrl = function(host, rootRelativeUrl, extension){
	return 'http://' + host + rootRelativeUrl + (extension || '');
};

var _getAbsoluteUrlResolver = function(req){
	return function(rootRelativeUrl, extension){
		return _toAbsoluteUrl(req.headers.host, rootRelativeUrl, extension);
	}
};

// render helper
function renderView(res, title, model) {
	var absoluteUrlResolver = _getAbsoluteUrlResolver(res.req);

	res.set('Cache-Control', 'no-cache, no-store');

	var links = [
	{
		rel: 'alternate',
		href: absoluteUrlResolver(res.req.url + '.json'),
		type: 'application/json',
		title: 'JSON format'
	},
	{
		rel: 'alternate',
		href: absoluteUrlResolver(res.req.url + '.html'),
		type: 'text/html',
		title: 'HTML format'
	}];
	// using this because the default link function in the ExpressJS Response object 
	// does not support adding parameters beyond rel
	headerLinking.setToResponse(res, links);

	var viewModel = {
		title: title,
		model: model,
		toAbsoluteUrl: absoluteUrlResolver
	}
	
	// use the built in content negotiation to select a format template
	res.format({
		'text/html': function() {
			viewModel.formatExtension = '.html';
			res.render('bugs-all-html', viewModel);	
		},

		'application/json': function() {
			viewModel.formatExtension = '.json';
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