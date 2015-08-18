app.controller("MahjongController", ['$scope', '$timeout', 'signalRHubProxy', 'mjService',
    function ($scope, $timeout, signalRHubProxy, mjService) {
        $scope.isDisonnected = true;
        $scope.gameIsReady = false;
        $scope.selectedTiles = [];
        $scope.isMyturn = false;
        $scope.record = {};
        $scope.canPickTile = false;

        var startup = function(conId){
            $scope.isDisonnected = !(conId != undefined);
            $scope.currentUserId = conId;
        };
        
        var clientPushHubProxy = signalRHubProxy(signalRHubProxy.defaultServer, 'gameHub', startup);

        clientPushHubProxy.on('showWinner', function (game) {
            $scope.record = game.Records[game.Records.length-1];
            updateOtherPlayer(game);
            updateGame(game);
            $("#myModal").modal('show');
        });

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

        var updateGame = function (game) {
            $scope.game = game;
            $scope.currentPlayer = mjService.getCurrentPlayer(game, $scope.currentUserId);
            $scope.currentPlayerWind = mjService.getWindName($scope.currentPlayer.Wind);            
            $scope.isMyturn = ($scope.currentUserId == game.PlayerTurn.ConnectionId)
            $timeout(function () {$scope.canPickTile = $scope.currentPlayer.CanPickTile}, 4000);            
            $scope.isRightPlayerTurn = ($scope.rightPlayer.ConnectionId == game.PlayerTurn.ConnectionId);
            $scope.isLeftPlayerTurn = ($scope.leftPlayer.ConnectionId == game.PlayerTurn.ConnectionId);
            $scope.isTopPlayerTurn = ($scope.topPlayer.ConnectionId == game.PlayerTurn.ConnectionId);
            $scope.lastTile = game.LastTile;
            $scope.currentGameWind = mjService.getWindName($scope.game.CurrentWind);
        }

        var updateOtherPlayer = function (game){
            mjService.setPlayer(game, $scope.currentUserId);
            $scope.topPlayer = mjService.topPlayer;
            $scope.rightPlayer = mjService.rightPlayer;
            $scope.leftPlayer = mjService.leftPlayer;
        }

        clientPushHubProxy.on('startGame', function (game) {
            mjService.setPlayer(game, $scope.currentUserId);
            $scope.topPlayer = mjService.topPlayer;
            $scope.rightPlayer = mjService.rightPlayer;
            $scope.leftPlayer = mjService.leftPlayer;
            updateGame(game);
        });

        clientPushHubProxy.on('startNextGame', function (game) {
            updateGame(game);
            $('#myModal').modal('hide');
        });

        clientPushHubProxy.on('updateGame', function (game) {
            updateGame(game);
        });

        clientPushHubProxy.on('updatePlayerCount', function (playerCount) {
            $scope.totalPlayers = playerCount;
        });

        clientPushHubProxy.on('alertUser', function (msg) {
            $scope.warningMessage = msg;
        });

        $scope.join = function () {
            var un = $('#usernameTB').val();
            clientPushHubProxy.invoke2('Join', un, 'mjbuddy', function () {
            });
        };

        $scope.resetGame = function () {
            clientPushHubProxy.invoke('ResetGame', function () { });
        };

        $scope.fnIsTileActive = function (tileId){
            return $.inArray(tileId, $scope.selectedTiles) > -1;
        }

        $scope.fnSelectTile = function (tileId, evt) {

            var isActive = evt.currentTarget.className.indexOf("activeTile") > -1

            if (isActive) {
                //remove tile
                $scope.selectedTiles = $.grep($scope.selectedTiles, function (e) {
                    return e !== tileId;
                });
            } else {
                $scope.selectedTiles.push(tileId)
            }
        };

        $scope.fnStartNextGame = function () {            
            clientPushHubProxy.invoke1('StartNextGame', 'mjbuddy', function () {});
        };

        $scope.fnPlayerMove = function (move, tiles) {
            $scope.warningMessage = "";
            clientPushHubProxy.invoke3('PlayerMove', 'mjbuddy', move, tiles, function (game) {
                $scope.$apply(function () {
                    $scope.game = game;
                })
            });
            //clear the selected tiles for every player move
            $scope.selectedTiles = [];
        };
    }
]);





