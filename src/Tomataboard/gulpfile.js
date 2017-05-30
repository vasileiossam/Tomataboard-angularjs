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
var debug = require('gulp-debug');

var paths = {
    webroot: "./wwwroot/"
};
paths.dashboardJsDest = paths.webroot + "js/dashboard.min.js";
paths.dashboardCssDest = paths.webroot + "css/dashboard.min.css";

// ------ clean -------

gulp.task("clean:js", function (cb) {
    rimraf(paths.dashboardJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.dashboardCssDest, cb);
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
            paths.webroot + "app/dashboard/*app.js",
            paths.webroot + "app/dashboard/**/*.js",
            paths.webroot + "app/common/**/*.js"],
        {
            base: "."
        })
        .pipe(concat(paths.dashboardJsDest))
        .pipe(ngAnnotate())
        .pipe(gulp.dest("."));
});

// minimize dashboard
gulp.task("min:css", function () {
    return gulp.src([
        paths.webroot + "css/dashboard.css",
        paths.webroot + "app/dashboard/**/*.css"])
        .pipe(concat(paths.dashboardCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

var getPublishJson = function () {
    return JSON.parse(fs.readFileSync('./publish.json', 'utf8'));
};

// bump version number
gulp.task("bump", function () {
    return gulp.src("./project.json")
    .pipe(bump())
    .pipe(gulp.dest("./"));
});

// set version number in html
gulp.task("version:html", ['bump'], function (callback) {
    gulp.src("./wwwroot/app/dashboard/settings/settings.html")
     .pipe(htmlreplace({
         'version': getPublishJson().version
     },
     {
         keepBlockTags: true
     }))
    .pipe(gulp.dest("./wwwroot/app/dashboard/settings/"));

    callback();
});

// ----------------------------
// prepublish tasks
// ----------------------------
gulp.task("version", function (callback) {
    runSequence('bump',
              'version:html',
              callback);
});
gulp.task("clean", ["clean:js", "clean:css", "clean:logfiles"]);
gulp.task("min", ["min:js", "min:css"]);