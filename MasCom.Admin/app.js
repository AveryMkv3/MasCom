const express = require("express");
const cookieParser = require("cookie-parser");
const path = require("path");
const session = require('express-session');
const bcrypt = require("bcrypt");
var adroutes = require('./routes/admin');
var usroutes= require('./routes/user');

const  app = express();



const port = process.env.PORT || 3000;


app.use(cookieParser());
app.use(express.urlencoded({ extended: false }));
app.use(express.json());
// app.use(express.static("static"));

app.use(express.static(path.join(__dirname, 'public')));

  
app.set('view engine', 'ejs');
app.set('views', path.join(__dirname, 'views'));

app.use(adroutes);
app.use(usroutes);

app.listen(port, (req, res) =>{
  console.log(`Server is running on http://localhost:${port}`);
});
