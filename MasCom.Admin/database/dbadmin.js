"use strict";
const sqlite3 = require('sqlite3').verbose();

class Db {
    constructor(file) {
        this.db = new sqlite3.Database(file);
        this.createTable()
    }

    createTable() {
        const sql = `
        CREATE TABLE IF NOT EXISTS "auth_admin" (
            "id"	integer ,
            "password"	varchar(128) ,
            "last_login"	datetime,
            "is_superuser"	bool DEFAULT 0 ,
            "username"	varchar(150)  UNIQUE,
            "first_name"	varchar(30) ,
            "email"	varchar(254) ,
            "is_staff"	bool DEFAULT 0 ,
            "is_active"	bool DEFAULT 0 ,
            "date_joined"	datetime ,
            "last_name"	varchar(150) ,
            PRIMARY KEY("id" AUTOINCREMENT)
        )`
        return this.db.run(sql);
    }

    selectByEmail(email, callback) {
        return this.db.get(
            `SELECT * FROM auth_admin WHERE email = ?`,
            [email],function(err,row){
                callback(err,row)
            })
    }

    selectByUsername(username, callback) {
        return this.db.get(
            `SELECT * FROM auth_admin WHERE username = ?`,
            [username],function(err,row){
                callback(err,row)
            })
    }
    selectById(id, callback) {
        return this.db.get(
            `SELECT * FROM auth_admin WHERE id = ?`,
            [id],function(err,row){
                callback(err,row)
            })
    }


    insertAdmin(auth_admin, callback) {
        return this.db.run(
            'INSERT INTO auth_admin (username,password,date_joined) VALUES (?,?,CURRENT_TIMESTAMP)',
            auth_admin, (err) => {
                callback(err)
            })
    }

    selectAll(callback) {
        return this.db.all(`SELECT * FROM auth_admin`, function(err,rows){
            callback(err,rows)
        })
    }

    insert(user, callback) {
        return this.db.run(
            'INSERT INTO user (name,lastname,username,email,user_pass) VALUES (?,?,?,?,?)',
            user, (err) => {
                callback(err)
            })
    }
    update(auth_admin, callback) {
        return this.db.run(
            'UPDATE auth_admin SET username = ? ,first_name =?, last_name = ?, email = ?, is_active = ?, is_staff =?, is_superuser = ?, last_login = ?, date_joined = ? WHERE id = ?',
            auth_admin, (err) => {
                callback(err)
            })
    }

    delete(id, callback) {
        return this.db.run(
            `DELETE FROM auth_admin WHERE id = ?`,
            id, (err) => {
                callback(err)
            })
    }

}

module.exports = Db