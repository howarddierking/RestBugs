var owin = require('connect-owin'),
    express = require('express');

var app = express();
//app.use(express.compress()); // combine .NET & node middlewares
app.use(owin({
    assemblyFile: __dirname + '\\bin\\Debug\\RestBugs.NodeHost.dll',
    basedir: __dirname + '\\bin\\Debug\\'
}));
app.listen(3000);
