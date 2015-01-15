﻿var express = require('express');
var fs = require('fs');
var http = require('http');
var request = require('request');
var log4js = require('log4js');
var _ = require('underscore');
var qs = require('querystring');
var sys = require('sys');
var exec = require('child_process').exec;
var Client = require('node-rest-client').Client;
var logger = log4js.getLogger();
var restClient = new Client();
var file = "appliance.json";
var requestToken = null;

var rosisTenantId = "4f043ef5887f46959d30545cf8b77b11";
var rosisServerAddress = "http://212.17.80.213";

var ourTenantId = "adeb2b7d0e1d41c29d88666551c5d903";
var ourServerAddress = "http://172.20.10.6";

var virtualMachines = [{
    "Id": "1",
    "ReferencedVirtualMachineId": "553c0069-14c2-45de-99d2-39b1610f3ea1",
    "Name": "Cirros",
    "Description": "Cirros",
    "Type": "Appliance",
    "ApplicationType": "",
    "OperatingSystem": "Linux",
    "OperatingSystemType": "Linux",
    "OperatingSystemVersion": "6.1",
    "Size": "400000",
    "RecommendedCPU": "1000",
    "RecommendedRAM": "1024",
    "SupportedVirtualizationPlatform": "",
    "Software": [
        "Virtual Box",
        "Microsoft Office"
    ],
    "SupportedProgramingLanguages": [
        "C#",
        "C++"
    ],
    "Rating": "5",
    "RatingDescription": "Usable for something",
    "Status": "Stopped"
}, {
    "Id": "2",
    "ReferencedVirtualMachineId": "62d52825-a2bf-4845-aeec-ad40e245f9fd",
    "Name": "CirrosII",
    "Description": "CirrosII",
    "Type": "Appliance",
    "ApplicationType": "",
    "OperatingSystem": "Linux",
    "OperatingSystemType": "Linuxe",
    "OperatingSystemVersion": "5.1",
    "Size": "400000",
    "RecommendedCPU": "2000",
    "RecommendedRAM": "2048",
    "SupportedVirtualizationPlatform": "",
    "Software": [
        "Virtual Box",
        "Microsoft Office",
        "Microsoft Visual Studio 2013"
    ],
    "SupportedProgramingLanguages": [
        "C#",
        "C++",
        "HTML"
    ],
    "Rating": "3",
    "RatingDescription": "Useable for anything",
    "Status": "Stopped"
}];

var start = function (id) {
    var machine = getMachine(id);
    if (machine) {
        machine.Status = "Started";
    }
};

var stop = function (id) {
    var machine = getMachine(id);
    if (machine) {
        machine.Status = "Stopped";
    }
};

var add = function(description) {
    var machine = getMachine(description.Id);
    if (machine) {
        logger.error("Cannot create virtual machine. A machine with the given id already exists.");
        return { Success: false, ErrorMessage: "Cannot create virtual machine. A machine with the given id already exists.", Data: null};
    } 
    logger.info("Adding new virtual machine");
    description.Rating = "";
    description.RatingDescription = "";
    description.Status = "Stopped";
    virtualMachines.push(description);
        return { Success: true, ErrorMessage: "", Data: virtualMachines };
};

var getMachines = function(operatingsystem, type) {
    var machines = [];
    _.each(virtualMachines, function(machine) {
        var match = false;
        if ((operatingsystem  && operatingsystem !== "all") && (type && type !== "all")) {
            if (machine.OperatingSystemType.indexOf(operatingsystem) > -1 && machine.Type.indexOf(type) > -1) {
                match = true;
            }
        } else if ((type && type.length > 0 && type !== "all") && (operatingsystem && operatingsystem === "all")) {
                if (machine.Type.indexOf(type) > -1) {
                    match = true;
                }
        } else if ((operatingsystem && operatingsystem !== "all") && (type && type === "all")) {
            if (machine.OperatingSystemType.indexOf(operatingsystem) > -1) {
                match = true;
            }
        } else if (type === "all" && operatingsystem === "all") {
            match = true;
        }
        if (match === true) {
            machines.push(machine);
        }
    });
    return machines;
};

var updateDescription = function (id, description) {
    var machine = getMachine(id);
    if (machine) {
        machine.Description = description;
        return { Success: true, ErrorMessage: "", Data: virtualMachines };
    } 
        logger.error("Could not find virutal machine for the given id");
        return { Success: false, ErrorMessage: "Could not find virutal machine for the given id", Data: null };
};

var updateRating = function(id, rating, comment) {
    var machine = getMachine(id);
    if (machine) {
        machine.Rating = rating;
        machine.RatingDescription = comment;
        return { Success: true, ErrorMessage: "", Data: virtualMachines };
    } 
        logger.error("Could not find virutal machine for the given id");
        return { Success: false, ErrorMessage: "Could not find virutal machine for the given id", Data: null };
};

var getMachine = function(id) {
    var machine = _.findWhere(virtualMachines, { Id: id });
    return machine;
};

var updateOperation = function (id, operation, res) {
    var machine = _.findWhere(virtualMachines, { Id: id });
    var rosisRequestData = JSON.stringify({
        "auth": {
            "tenantName": "admin",
            "passwordCredentials": {
                "username": "admin",
                "password": "Openstack#2014"
            }
        }
    });
    var ourRequestData = JSON.stringify({
        "auth": {
            "tenantName": "admin",
            "passwordCredentials": {
                "username": "admin",
                "password": "supersecret"
            }
        }
    });
    var startData = null;
    if (operation === "Start") {
        startData = JSON.stringify({ "os-start": null });
    } else {
        startData = JSON.stringify({ "os-stop": null });
    }

    request.post({
        headers: { 'content-type': 'application/json' },
        //url: 'http://212.17.80.213:5000/v2.0/tokens',
        url: ourServerAddress + ":5000/v2.0/tokens",
        //body: rosisRequestData
        body: ourRequestData
    }, function (error, response, body) {
        var jsonObject = JSON.parse(body);
        requestToken = jsonObject.access.token;
        request.post({
            headers: { 'content-type': 'application/json', "X-Auth-Token": requestToken.id, 'Content-Length': startData.length },
            //url: 'http://212.17.80.213:8774/v2/4f043ef5887f46959d30545cf8b77b11/servers/590583b4-6a0b-43a5-bb93-ac464d148b00/action',
            url: ourServerAddress +':8774/v2/' + ourTenantId + "/servers/" + machine.ReferencedVirtualMachineId + "/action",
            //url: 'http://172.20.10.6:8774/v2/adeb2b7d0e1d41c29d88666551c5d903/servers/553c0069-14c2-45de-99d2-39b1610f3ea1/action',
            body: startData
        }, function (errorI, responseI, bodyI) {
            var jsonObjectI = null;
            if(bodyI &&  body.length > 0){
                jsonObjectI = JSON.parse(bodyI);
            }
            if (jsonObjectI && jsonObjectI.conflictRequest) {
                machine.operation = "Started";
            }else{
                if (operation === "Start") {
                    machine.Status = "Started";
                } else {
                    machine.Status = "Stopped";
                }
            }
            res.send( { Success: true, ErrorMessage: "", Data: virtualMachines });
        });
    });
   
};

var app = express();

/** Download a virtual machine **/
app.get('/download/:id', function (request, response) {
    if (request.params.id) {
        var machine = getMachine(request.params.id);
        if (machine) {
            var stream = fs.createWriteStream(file);
            stream.once('open', function (fd) {
                stream.write(JSON.stringify(machine));
                stream.end();
                response.download(file);
            });
        } else {
            logger.error("Could not find virtual machine or appliance for id");
            response.send({ Success: false, ErrorMessage: "Could not find virtual machine or appliance for id", Data: null});
        }
    } else {
        logger.error("No id supplied. Error");
        response.send({ Success: false, ErrorMessage: "No id supplied. Error", Data: null });
    }
});

/** List all virtual machines **/
app.get('/machines', function (request, response) {
    logger.info("Received 'List all Virtual Machines' request");
    response.send({ Success: true, ErrorMessage: "", Data: virtualMachines });
});

/** Search for specific virtual machines by operating system and software **/
app.get('/machine/:operatingsystem/:type', function (request, response) {
    logger.info("Received 'List Virtual Machines by operating syste and software' request");
    var machines = getMachines(request.params.operatingsystem, request.params.type);
    response.send({ Success: true, ErrorMessage: "", Data: machines });
});

/** Add a new virtual machine **/
app.post('/machine', function (request, response) {
    logger.info("Received 'Add new Virtual Machine' request");
    console.log(request.body);
    if (request.method == 'POST') {
        var body = '';
        request.on('data', function (data) {
            body += data;

            if (body.length > 1e6)
                request.connection.destroy();
        });
        request.on('end', function () {
            try {
                var jsonObject = JSON.parse(body);
                var parsedObject = JSON.parse(jsonObject);
                var vmResponse = add(parsedObject);
                response.send(vmResponse);
            } catch (e) {
                response.send({ Success: false, ErrorMessage: "Could not add virutal machine. Please try it again.", Data: null });
            }    
        });
    }else{
        response.send({ Success: false, ErrorMessage: "Could not add virutal machine. Please try it again.", Data: null});
    }
});

/** Start or stop a virtual machine **/
app.post('/machine/state/:id/:operation', function(request, response) {
    logger.info("Received 'Operation for Virtual Machine' request");
    var opResponse = updateOperation(request.params.id, request.params.operation, response);
    //response.send(opResponse);
});

/** Change the description of a virtual machine **/
app.post('/machine/:id/:description', function (request, response) {
    logger.info("Received 'Update Description for Virtual Machiner' request");
    var desResponse = updateDescription(request.params.id, request.params.description);
    response.send(desResponse);
});

/** Add a rating with a comment to the virtual machine **/
app.post('/machine/:id/:rating/:comment', function (request, response) {
    logger.info("Received 'Update Rating for Virtual Machine' request");
    var ratResponse = updateRating(request.params.id, request.params.rating, request.params.comment);
    response.send(ratResponse);
});

var port = 1337;
var server = app.listen(port, function () {
    var host = server.address().address;
    var port = server.address().port;
    logger.info('Express server listening on listening at http://%s:%s', host, port);
});