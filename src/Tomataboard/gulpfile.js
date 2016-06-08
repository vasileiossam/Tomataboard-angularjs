/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    fs = require('fs'),

    // add angularjs dependency injection annotations 
    ngAnnotate = require("gulp-ng-annotate");

// version bumping
var bump = require("gulp-bump");

// replace build blocks in HTML
var htmlreplace = require("gulp-html-replace");

// run a series of dependent gulp tasks in order
// This is intended to be a temporary solution until the release of gulp 4.0 
var runSequence = require('run-sequence');

var del = require("del");

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";

paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";

paths.concatJsDest = paths.webroot + "js/dashboard.min.js";
paths.concatCssDest = paths.webroot + "css/dashboard.min.css";

// ------ clean -------

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});
 
gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

// https://github.com/gulpjs/gulp/blob/master/docs/recipes/delete-files-folder.md
gulp.task("clean:logfiles", function () {
    return del([
       paths.webroot + "*.log"
    ]);
});

// ------ minification  -------

// minimize dashboard
gulp.task("min:js", function () {
    return gulp.src([
            // include angular apps first
            paths.webroot + "js/*app.js",
            paths.js,
            paths.webroot + "app/**/*.js",
            "!" + paths.minJs],
        {
            base: "." 
            
        })
        .pipe(concat(paths.concatJsDest))
        .pipe(ngAnnotate())
       // .pipe(uglify())
        .pipe(gulp.dest("."));
});

// minimize dashboard
gulp.task("min:css", function () {
    return gulp.src([paths.css, paths.webroot + "app/**/*.css",
        "!" + paths.webroot + "css/**/site.css",
        "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

var getProjectJson = function () {
    return JSON.parse(fs.readFileSync('./project.json', 'utf8'));
};

// bump version number
gulp.task("bump", function () {
    return gulp.src("./project.json")
    .pipe(bump())
    .pipe(gulp.dest("./"));
});

// set version number in html
gulp.task("version:html", ['bump'], function (callback) {
    gulp.src("./wwwroot/app/settings/settings.html")
     .pipe(htmlreplace({
         'version': getProjectJson().version
     },
     {
         keepBlockTags: true
     }))
    .pipe(gulp.dest("./wwwroot/app/settings/"));

    callback();
});

// ----------------------------
// prepublish tasks
// ----------------------------
gulp.task("version", function(callback) {
    runSequence('bump',
              'version:html',
              callback);
              });
gulp.task("clean", ["clean:js", "clean:css", "clean:logfiles"]);
gulp.task("min", ["min:js", "min:css"]);