var DropdownControl = (function () {
    function DropdownControl(ctr, url, cb) {
        this.ctr = null;
        this.url = null;
        this.changecallback = null;
        this.ctr = ctr;
        this.url = url;
        this.changecallback = cb;
    }
    DropdownControl.prototype.update = function () {
        var id = this.ctr.attr('id');
        var $html = '<select class="form-control" id = "' + id + '" name = "' + id + '" ><option selected="selected" disabled="true">--Please Select --</option>< / select>';
        var self = this;
        this.ctr.replaceWith($html);
        this.ctr = $('#' + id);
        this.ctr.change(this.changecallback);
        $.getJSON(this.url()).done(function (res) {
            $.each(res, function (i, o) {
                self.ctr.append($('<option value="' + o + '">' + o + '</option>'));
            });
        });
    };

    DropdownControl.prototype.reset = function () {
        this.ctr.val(null);
    };

    DropdownControl.prototype.value = function () {
        return this.ctr.val();
    };
    return DropdownControl;
})();

var CreateNewPage = (function () {
    function CreateNewPage(subscriptionCtrl, serviceCtrl, deploymentCtrl, virtualMachineCtrl) {
        var _this = this;
        this.subscriptionCtrl = null;
        this.serviceCtrl = null;
        this.deploymentCtrl = null;
        this.virtualMachineCtrl = null;
        this.subscriptionCtrl = subscriptionCtrl;
        this.serviceCtrl = new DropdownControl(serviceCtrl, function () {
            return '/api/HostedServices/' + subscriptionCtrl.val();
        }, function () {
            _this.deploymentCtrl.reset();
            _this.virtualMachineCtrl.reset();
            _this.deploymentCtrl.update();
        });
        this.deploymentCtrl = new DropdownControl(deploymentCtrl, function () {
            return '/api/HostedServices/' + subscriptionCtrl.val() + '/' + _this.serviceCtrl.value();
        }, function () {
            _this.virtualMachineCtrl.reset();
            _this.virtualMachineCtrl.update();
        });
        this.virtualMachineCtrl = new DropdownControl(virtualMachineCtrl, function () {
            return '/api/HostedServices/' + subscriptionCtrl.val() + '/' + _this.serviceCtrl.value() + '/' + _this.deploymentCtrl.value();
        }, function () {
        });

        this.subscriptionCtrl.change(function () {
            _this.serviceCtrl.reset();
            _this.deploymentCtrl.reset();
            _this.virtualMachineCtrl.reset();
            _this.serviceCtrl.update();
        });

        this.serviceCtrl.update();
    }
    return CreateNewPage;
})();

var page = new CreateNewPage($('#SubscriptionId'), $('#ServiceName'), $('#DeploymentName'), $('#VirtualMachineName'));
//# sourceMappingURL=VMSchedule.js.map
