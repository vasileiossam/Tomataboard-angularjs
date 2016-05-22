/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    ngAnnotate = require("gulp-ng-annotate");

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

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});
 
gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css", "clean:logfiles"]);

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

gulp.task("min", ["min:js", "min:css"]);

// https://github.com/gulpjs/gulp/blob/master/docs/recipes/delete-files-folder.md
gulp.task("clean:logfiles", function() {
     return del([
        paths.webroot  + "*.log"
    ]); 
});