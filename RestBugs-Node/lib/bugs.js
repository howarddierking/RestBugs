var _ = require('underscore');

var _historyRecord = function(comments, changes){
	return {
		addedOn: new Date(),
		comments: comments,
		changes: changes	
	};
};

var _updateStatus = function(status){
	return function(comments){
		if(status !== this.status) {
			this.status = status;
			this.history.push(_historyRecord(comments, { status: this.status }));
		}

		return this;
	};	
};

exports.from = function(obj){
	// attach methods
	obj.backlog = _updateStatus('Backlog');
	obj.working = _updateStatus('Working');
	obj.qa = _updateStatus('QA');
	obj.done = _updateStatus('Done');

	return obj;
};

exports.create = function(title, description){
	var bug = exports.from({ 
		title: title,
		description: description,
		status: undefined,
		history: []
	});

	bug.backlog('bug created');

	return bug; 
}