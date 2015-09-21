app.factory('signalRHubProxy', ['$rootScope', 

        function ($rootScope) {

            function signalRHubProxyFactory(serverUrl, hubName, startup) {

                var connection = $.hubConnection();
                var proxy = connection.createHubProxy(hubName);
                connection.start().done(function () {
                    $rootScope.$apply(function () {
                        if (startup) {
                            startup(connection.id);
                        }
                    });                    
                });

                return {
                    on: function (eventName, callback) {
                        proxy.on(eventName, function (result) {
                            $rootScope.$apply(function () {
                                if (callback) {
                                    callback(result);
                                }
                            });
                        });
                    },
                    on1: function (eventName, callback) {
                        proxy.on(eventName, function (result1, result2) {
                            $rootScope.$apply(function () {
                                if (callback) {
                                    callback(result1, result2);
                                }
                            });
                        });
                    },
                    off: function (eventName, callback) {
                        proxy.off(eventName, function (result) {
                            $rootScope.$apply(function () {
                                if (callback) {
                                    callback(result);
                                }
                            });
                        });
                    },
                    invoke: function (methodName, callback) {
                        proxy.invoke(methodName)
                            .done(function (result) {
                                $rootScope.$apply(function () {
                                    if (callback) {
                                        callback(result);
                                    }
                                });
                            });
                    },
                    invoke1: function (methodName, arg, callback) {
                        proxy.invoke(methodName, arg)
                            .done(function (result) {
                                $rootScope.$apply(function () {
                                    if (callback) {
                                        callback(result);
                                    }
                                });
                            });
                    },
                    invoke2: function (methodName, arg1, arg2, callback) {
                        proxy.invoke(methodName, arg1, arg2)
                            .done(function (result) {
                                $rootScope.$apply(function () {
                                    if (callback) {
                                        callback(result);
                                    }
                                });
                            });
                    },
                    invoke3: function (methodName, arg1, arg2, arg3, callback) {
                        proxy.invoke(methodName, arg1, arg2, arg3)
                            .done(function (result) {
                                $rootScope.$apply(function () {
                                    if (callback) {
                                        callback(result);
                                    }
                                });
                            });
                    },
                    connection: connection
                };
            };

            return signalRHubProxyFactory;
        }]);