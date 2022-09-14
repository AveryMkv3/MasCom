var express = require('express');
var router = express.Router();
const bcrypt = require("bcrypt");
const session = require('express-session');

const DB = require("../database/dbadmin");
const db = new DB("Mascom.db");


router.use(session({secret: 'salib'}));


router.get('/', function(req, res){
    res.redirect('/login');
});
router.get('/login', function(req, res){
    res.render('login', {error: ''});
})
.post('/login', (req, res) =>{
    const username = req.body.username;
    const password = req.body.password;
    db.selectByUsername(username, (err, user) => {
        if ( err ) return res.status(500).render('login', {error: 'Error de serveur'});
        if ( !user ) return res.status( 404 ).render('login', {error : 'No user found'});
        let passwordIsValid = bcrypt.compareSync( password, user.password );
        if ( !passwordIsValid ) return res.status( 401 ).render('login', {error: 'password invalid'});
        req.session.userid = user.username;
        res.redirect('/home');
    })

});

router.get('/home', (req, res) => {
    if (req.session.userid){
        console.log(req.session);
        res.render('home');
    } else {
        res.redirect('/')
    }
});

router.get('/register-admin', function(req, res){
    res.render('register-admin');
});
router.post('/register-admin', function(req, res) {
    if(req.body.password == req.body.cpass){
        db.insertAdmin([
            req.body.username,
            bcrypt.hashSync(req.body.password, 8)
        ],
        function(err){
        if ( err ) return res.status(500).send("Server error")
        db.selectByUsername(req.body.username, (err, auth_user) => {
            if ( err ) return res.status( 500 ).send( "There was a problem getting user" )
            const sm = "Utilisateurs a été enregistré";
            console.log(sm);
            res.redirect('/adhome'); 
        })  
        }
        )
    } else {
        res.send('incorrectes')
    }
});

router.get('/change/:id', function(req, res){
    const id = req.params.id;
    db.selectById(id, (err, user) =>{
        if ( err ) return res.status( 500 ).send("There was a problem registering the user");
        res.render('change', {users: user});
         
    })

})

router.get('/adhome', function(req, res){
    db.selectAll((err, auth_user) =>{
        if ( err) return res.status(500 ).send("There was a problem registering the user")
        res.render('adhome', {admin : auth_user});
    })
});

router.post('/change/:id', function(req, res){
    const id = req.params.id;
    const username = req.body.username;
    const firstname = req.body.firstname;
    const lastname = req.body.lastname;
    const email = req.body.email;
    const is_active = req.body.is_active ? true: false;
    const is_staff = req.body.is_staff ? true: false;
    const is_superuser = req.body.is_superuser ? true: false;
    const last_login = req.body.last_login;
    const date_joined = req.body.date_joined;

    db.update([
        username,
        firstname,
        lastname,
        email,
        is_active,
        is_staff,
        is_superuser,
        last_login,
        date_joined,
        id,
    ],
        function(err){
            if ( err ) return res.status( 500 ).send( "There was a problem registering the user." )
            db.selectById(id, (err, user)=>{
                if( err ) return res.status( 500 ).send("There was getting.")
                res.redirect('/adhome')
            })
        }
    )


});

router.get('/delete/:id', function(req, res){
    const id = req.params.id;
    db.selectById(id, (err, auth_user) =>{
        if( err ) return res.status( 500 ).send("There was a problem registering the user.")
        res.render('delete', {users: auth_user})
    })

});

router.post('/delete/:id', function(req, res){
    const id = req.params.id;
    db.delete([
        id,
    ],
        function(err){
            db.selectById(id, (err, user) => {
                if( err ) return res.status( 500 ).send("There was a problem registering the user.")
                console.log("users deleted");
                res.redirect('/adhome');
            })
        }
    )
});

router.get('/logout', function(req, res){
    if(req.session.userid){
        delete req.session.userid;
        res.redirect('/');

    }else{
        res.redirect('/home');
    }
})

module.exports = router;