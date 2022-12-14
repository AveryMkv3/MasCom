"use strict";
const sqlite3 = require('sqlite3').verbose();

class Db {
    constructor(file) {
        this.db = new sqlite3.Database(file);
        this.createTable()
    }

    createTable() {
        const sql = `
            CREATE TABLE IF NOT EXISTS user (
                id integer PRIMARY KEY,
                name text,
                lastname text,
                username text UNIQUE,
                email text UNIQUE,
                passwordhash text)`
        return this.db.run(sql);
    }

    selectByEmail(email, callback) {
        return this.db.get(
            `SELECT * FROM user WHERE email = ?`,
            [email],function(err,row){
                callback(err,row)
            })
    }

    selectById(id, callback) {
        return this.db.get(
            `SELECT * FROM user WHERE id = ?`,
            [id],function(err,row){
                callback(err,row)
            })
    }



    selectAll(callback) {
        return this.db.all(`SELECT * FROM user`, function(err,rows){
            callback(err,rows)
        })
    }

    insert(user, callback) {
        return this.db.run(
            'INSERT INTO user (name,lastname,username,email,passwordhash) VALUES (?,?,?,?,?)',
            user, (err) => {
                callback(err)
            })
    }
    update(user, callback) {
        return this.db.run(
            'UPDATE user SET name = ? ,lastname =?, username=?, email = ? WHERE id = ?',
            user, (err) => {
                callback(err)
            })
    }

    delete(id, callback) {
        return this.db.run(
            `DELETE FROM user WHERE id = ?`,
            id, (err) => {
                callback(err)
            })
    }

}

module.exports = Db