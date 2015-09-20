var archiveApp = angular.module('archiveApp', ['infinite-scroll']);

archiveApp.controller('DemoController', function ($scope, Archive) {
    $scope.archive = null;
    $scope.query = '';

    $scope.$watch("query", function (newValue, oldValue) {
        $scope.archive = new Archive(newValue);
        $scope.archive.more();
    });
});

// constructor function to encapsulate HTTP and pagination logic
archiveApp.factory('Archive', function ($http) {
    var Archive = function (query) {
        this.items = [];
        this.atEnd = false;
        this.busy = false;
        this.query = query;
        this.offset = 0;
        this.size = 10;
        this.error = false;

        if (query.length === 0)
            this.atEnd = true;
    };

    Archive.prototype.more = function () {
        if (this.atEnd) return;
        if (this.busy) return;
        this.busy = true;

        var url = "http://localhost:55039//api/message?q=" + this.query + "&from=" + this.offset + "&size=" + this.size;
        $http.get(url).then(
            function (response) {
                for (var i = 0; i < response.data.length; i++) {
                    var newItem = response.data[i];
                    newItem.Date = moment(newItem.Date); // wrap in Moment object
                    this.items.push(newItem);
                }
                this.offset += response.data.length - 1;
                this.atEnd = response.data.length < (this.size - 1);
                this.busy = false;
            }.bind(this),
            function (errorResponse) {
                debugger;
                this.error = true;
            }.bind(this));
    };

    return Archive;
});