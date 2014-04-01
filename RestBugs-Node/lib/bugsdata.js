var bugs = require('./bugs'),
	MongoClient = require('mongodb').MongoClient,
	ObjectID = require('mongodb').ObjectID,
	_ = require('underscore'),
	async = require('async');

var mongoConnectionString = process.env.RESTBUGS_DB || 'mongodb://127.0.0.1/restbugs',
	mongoConnectionOptions = {
		db: { native_parser: true }
	},
	bugsCollection = 'bugs';

// lazily create the MongoClient if it doesn't already exist
var dbClient;
var ensureDbClient = function(callback){
	if(dbClient) return callback(null, dbClient);
	MongoClient.connect(mongoConnectionString, mongoConnectionOptions, function(err, db){
		if(err){
			debugger;
			return callback(err, null);
		}

		dbClient = db;
		callback(null, dbClient);
	});
};

// exports ///////////////////////////////////////////////////////////////////////

var statusList = function(status){
	return function(callback){
		ensureDbClient(function(err, db){
			if(err) return callback(err, null);
			db.collection(bugsCollection).find({ status: 'Backlog' }).toArray(callback);
		});
	};
}

exports.getBug = function(bugID, callback){
	ensureDbClient(function(err, db){
		if(err) return callback(err, null);
		db.collection(bugsCollection).findOne({ _id: new ObjectID(bugID) }, function(err, data){
			if(err) return callback(err, null);
			callback(null, bugs.from(data));
		});
	});
};

exports.saveBug = function(bug, callback){
	var bugID = bug._id && new ObjectID(bug._id.toString());

	ensureDbClient(function(err, db){
		if(err) return callback(err, null);
		
		db.collection(bugsCollection).update({ _id: bugID }, bug, { upsert: true, w: 1 }, function(err, data){
			if(err) return callback(err, null);
			callback(null, bugs.from(data));
		})
	});
};

exports.lists = {};

exports.lists.backlog = function(callback){
	ensureDbClient(function(err, db){
		if(err) return callback(err, null);
		db.collection(bugsCollection).find({ status: 'Backlog' }).toArray(callback);
	});
};

exports.lists.working = function(callback){
	ensureDbClient(function(err, db){
		if(err) return callback(err, null);
		db.collection(bugsCollection).find({ status: 'Working' }).toArray(callback);
	});
};

exports.lists.qa = function(callback){
	ensureDbClient(function(err, db){
		if(err) return callback(err, null);
		db.collection(bugsCollection).find({ status: 'QA' }).toArray(callback);
	});
};

exports.lists.done = function(callback){
	ensureDbClient(function(err, db){
		if(err) return callback(err, null);
		db.collection(bugsCollection).find({ status: 'Done' }).toArray(callback);
	});
};