app.controller("MahjongController", ['$scope', 'signalRHubProxy', 'mjService',
    function ($scope, signalRHubProxy, mjService) {
        $scope.isDisonnected = true;
        $scope.gameIsReady = false;
        $scope.selectedTileId = 0;
        $scope.isMyturn = false;
        var startup = function(conId){
            $scope.isDisonnected = !(conId != undefined);
            $scope.currentUserId = conId;
        };
        var clientPushHubProxy = signalRHubProxy(signalRHubProxy.defaultServer, 'gameHub', startup);

        clientPushHubProxy.on('notifyUserInGroup', function (msg) {
            $('#gameStatus').append(msg + "<br/>");
        });
        
        clientPushHubProxy.on('playerJoined', function (user) {
            $('#usernameTB').attr('disabled', 'disabled');
            $('#join').attr('disabled', 'disabled');
        });

        clientPushHubProxy.on('waitingList', function (msg) {
            $('#gameStatus').append(msg + "<br/>");
        });

        clientPushHubProxy.on('gameStarted', function () {
            $scope.gameIsReady = true;
            $('#gameStatus').append("Game Started!  <br/>");
            $('#GameRoom').show();
            $('#WaitingRoom').hide();
        });

        clientPushHubProxy.on('startGame', function (game) {
            $scope.game = game;
            $scope.currentPlayer = mjService.getCurrentPlayer(game, $scope.currentUserId);
            $scope.currentPlayerWind = mjService.getWindName($scope.currentPlayer.Wind);
            $scope.isMyturn = ($scope.currentUserId == game.WhosTurn);

            mjService.setPlayer(game, $scope.currentUserId);
            $scope.topPlayer = mjService.topPlayer;
            $scope.rightPlayer = mjService.rightPlayer;
            $scope.leftPlayer = mjService.leftPlayer;
        });

        clientPushHubProxy.on('updateGame', function (game) {
            $scope.game = game;
            $scope.currentPlayer = mjService.getCurrentPlayer($scope.game, $scope.currentUserId);
            $scope.currentPlayerWind = mjService.getWindName($scope.currentPlayer.Wind);
            $scope.isMyturn = ($scope.currentUserId == game.WhosTurn);
            $scope.lastTile = game.LastTile;
        });

        clientPushHubProxy.on('alertUser', function (msg) {
            $scope.warningMessage = msg;
        });

        $scope.join = function () {
            var un = $('#usernameTB').val();
            clientPushHubProxy.invoke2('Join', un, 'mjbuddy', function () {
            });
        };
        $scope.fnSelectTile = function (tileId) {
            $scope.selectedTileId = tileId;
        };

        $scope.fnPlayerMove = function (move, tileId) {
            clientPushHubProxy.invoke3('PlayerMove', 'mjbuddy', move, tileId, function (game) {
                $scope.$apply(function () {
                    $scope.game = game;
                })
            });
        };
    }
]);





