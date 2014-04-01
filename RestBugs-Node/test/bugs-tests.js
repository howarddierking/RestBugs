var bugs = require('../lib/bugs'),
	_ = require('underscore'),
	util = require('util'),
	should = require('should');

describe('when creating a new bug', function(){
	var b = bugs.create('title', 'description');

	it('should be in the backlog state', function(){
		b.status.should.equal('Backlog');
	});
	it('should have 2 items in history', function(){
		b.history.length.should.equal(1);
	});

	it('should have 1 history item for creation', function(){
		b.history[0].changes.should.eql({ status: 'Backlog' });
		b.history[0].comments.should.equal('bug created');
	});
});

var executeStateChange = function(state){
	return function(){
		var bug = bugs.create('title', 'description')[state.transition](state.comments);

		it(util.format('should be in the %s state', state.status), function(){
			bug.status.should.equal(state.status);
		});

		it('should have an additional history item recording the change', function(){
			bug.history.length.should.equal(2);
		});
		it('should have last item in history reflect the change', function(){
			_.last(bug.history).comments.should.equal(state.comments);
			_.last(bug.history).changes.should.eql({ status: state.status });
		});
	};
};

describe('workflow state changes', function(){
	
	var states = [ 
		{ transition: 'working', 	status: 'Working',	comments: 'moved to working' }, 
		{ transition: 'qa',			status: 'QA',		comments: 'moved to qa' }, 
		{ transition: 'done',		status: 'Done',		comments: 'all done!' }];
	
	_.each(states, function(state){
		describe(util.format('move to %s', state.status), executeStateChange(state));
	});
});

describe('when moving to the state that the bug is already in', function(){
	it('should have the same values of the object prior to the operation', function(){
		var bug = bugs.create('title', 'description').backlog('moved to backlog');
		bug.history.length.should.equal(1);
	});
});