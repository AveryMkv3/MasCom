var express = require('express');
var router = express.Router();
const bcrypt = require("bcrypt");
const session = require('express-session');

const DB = require("../database/dbuser");
const { route } = require('./admin');
const db = new DB("Mascom.db");


router.get('/ushome', function(req, res){
    db.selectAll(( err, users ) => {
        if ( err ) return res.status( 500 ).send( "There was a problem getting datas" )
        // res.status( 200 ).send( users );
        res.render('ushome', {usermodel: users});

    }) 
});

router.get('/register', function(req, res){
    res.render('register')
});

router.post('/register', function(req, res){
    const name = req.body.name;
    const lastname = req.body.lastname;
    const username = req.body.username;
    const email = req.body.email;
    const passwordhash = req.body.passwordhash;
    db.insert( [
        name,
        lastname,
        username,
        email,
        bcrypt.hashSync( passwordhash, 8 )
    ],
        function ( err ) {
            if ( err ) return res.status( 500 ).send( "There was a problem registering the user." )
            db.selectByEmail( email, ( err, user ) => {
                if ( err ) return res.status( 500 ).send( "There was a problem getting user" )
                res.redirect('/ushome')
            } );
        } 
    );
});


router.get('/uschange/:id', function(req, res){
    const id = req.params.id;
    db.selectById(id, (err, user) =>{
        if( err ) return res.status( 500 ).send("There was a problem registering the user.")
        res.render('uschange', {users: user})
    })
});

router.post('/uschange/:id', function(req, res){
    const id = req.params.id;
    const name = req.body.name;
    const lastname = req.body.lastname;
    const username = req.body.username;
    const email = req.body.email;
    db.update([
        name,
        lastname,
        username,
        email,
        id,
    ],
        function(err){
            if ( err ) return res.status( 500 ).send( "There was a problem registering the user." )
            db.selectById(id, (err, user) =>{
                if( err ) return res.status( 500 ).send("There was getting.")
                res.redirect('/ushome')
            })
        }
    
    )

});


router.get('/usdelete/:id', function(req, res){
    const id = req.params.id;
    db.selectById(id, (err, user) =>{
        if( err ) return res.status( 500 ).send("There was a problem registering the user.")
        res.render('usdelete', {users: user})
    })
});

router.post('/usdelete/:id', function(req, res){
    const id = req.params.id;
    db.delete([
        id,
    ],
        function(err){
            db.selectById(id, (err, user) =>{
                if( err ) return res.status( 500 ).send("There was a problem registering the user.")
                res.redirect('/ushome');
            }) 
        }
    )

});


module.exports = router;