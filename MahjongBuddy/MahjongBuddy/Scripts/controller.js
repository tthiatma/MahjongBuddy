app.controller("MahjongController", ['$scope', '$timeout', 'signalRHubProxy', 'mjService',
    function ($scope, $timeout, signalRHubProxy, mjService) {

    //list of tiles that user clicked/selected
        $scope.selectedTiles = [];
    //list of tiles that user throw to board / board graveyard tiles
        $scope.boardTiles = [];
    //record to show history of the game
        $scope.record = {};
    //before the game end, there will be a countdown so that other player have a chance to declare win if the last tile is what they need
        $scope.hideGameEndCountdown = true;

        $scope.canPickTile = false;
        $scope.isMyturn = false;
        $scope.isDisonnected = true;
        $scope.gameIsReady = false;
        $scope.currentPlayer = null;
        $scope.dealer = null;
        $scope.gameEndTimeout = null;
        $scope.tenSectimeout = null;
        $scope.hideAlert = true;

    //func to allow ng-repeat works with int
        $scope.fnGetNumber = function (num) {
            return new Array(num);
        }       
        $scope.fnJoinGame = function () {
            var un = $('#usernameTB').val();
            clientPushHubProxy.invoke2('Join', un, 'mjbuddy', function (){});
        };
        $scope.fnResetGame = function () {
            clientPushHubProxy.invoke('ResetGame', function () {});
        };
        $scope.fnIsTileSelected = function (tileId) {
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
            clientPushHubProxy.invoke1('StartNextGame', 'mjbuddy', function (){});
        };
        $scope.fnPlayerMove = function (move, tiles) {
            $scope.warningMessage = "";
            clientPushHubProxy.invoke3('PlayerMove', 'mjbuddy', move, tiles, function (game) {});
            //clear the selected tiles for every player move
            $scope.selectedTiles = [];
        };
    //func that control drag and drop / sorting tile
        $scope.onDropComplete = function (index, obj, evt) {

            var targetTile = fnFindTileByIndex($scope.currentPlayer, index);

            targetTile.ActiveTileIndex = obj.ActiveTileIndex;
            obj.ActiveTileIndex = index;

            $scope.currentPlayer.IsTileAutoSort = false;
            $scope.selectedTiles = [];
        };
        $scope.fnToggleSortTile = function () {

        };


///**************************        
//Private Function section
///**************************
        var fnFindTileByIndex = function (currentPlayer, tileIndex) {
            var ret = null;
            $.each(currentPlayer.ActiveTiles, function (idx, val) {
                if (val.ActiveTileIndex == tileIndex) {
                    ret = val;
                }
            });
            return ret;
        }
        var fnStartup = function (conId) {
            $scope.isDisonnected = !(conId != undefined);
            $scope.currentUserId = conId;
        };
        var fnCancelGameEndCountDown = function () {
            // we stop the countdown to show the shownowinner modal
            if ($scope.gameEndTimeout != null && $scope.gameEndTimeout != undefined) {
                $timeout.cancel($scope.gameEndTimeout);
            }
            $scope.hideGameEndCountdown = true;
            $scope.gameEndCountdown = 10;
        }
        var fnUpdatePlayer = function (currentPlayer) {
            $scope.currentPlayer = currentPlayer            
            $scope.currentPlayerWind = mjService.getWindName(currentPlayer.Wind);
            $scope.isMyturn = ($scope.currentUserId == $scope.dealer.PlayerTurn);
            if ($scope.isMyturn) {
                $timeout(function () { $scope.canPickTile = $scope.currentPlayer.CanPickTile }, 5000);
            }
            $scope.isRightPlayerTurn = ($scope.currentPlayer.RightPlayer.ConnectionId == $scope.dealer.PlayerTurn);
            $scope.isLeftPlayerTurn = ($scope.currentPlayer.LeftPlayer.ConnectionId == $scope.dealer.PlayerTurn);
            $scope.isTopPlayerTurn = ($scope.currentPlayer.TopPlayer.ConnectionId == $scope.dealer.PlayerTurn);

        }

///**************************        
//clientPushHubProxy section
///**************************
        var clientPushHubProxy = signalRHubProxy(signalRHubProxy.defaultServer, 'gameHub', fnStartup);
        clientPushHubProxy.on('gameStarted', function () {
            $scope.gameIsReady = true;
            $('#gameStatus').append("Game Started!  <br/>");
            $('#GameRoom').show();
            $('#WaitingRoom').hide();
        });

    ///-------------------------
    ///Lobby
    ///-------------------------

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
        clientPushHubProxy.on('updatePlayerCount', function (playerCount) {
            $scope.totalPlayers = playerCount;
        });

    ///-------------------------
    ///In Game
    ///-------------------------
        
    //dealer holds critical information about the game, this needs to be populated first (even before starting the game)
        clientPushHubProxy.on('updateDealer', function (dealer) {
            $scope.dealer = dealer;
        });
        clientPushHubProxy.on('startGame', function (playerInfo) {
            $scope.currentGameWind = mjService.getWindName(dealer.CurrentWind);
            fnUpdatePlayer(playerInfo);
        });
        clientPushHubProxy.on('startNextGame', function (game) {
            $scope.currentGameWind = mjService.getWindName(dealer.CurrentWind);
            $('#showWinnerModal').modal('hide');
            $('#showNoWinnerModal').modal('hide');
        });
        clientPushHubProxy.on('updateCurrentPlayer', function (playerInfo) {
            fnUpdatePlayer(playerInfo);
        });
        clientPushHubProxy.on('addBoardTiles', function (tile) {
            if (tile != null) { $scope.boardTiles.push(tile); }
        });
        clientPushHubProxy.on('alertUser', function (msg) {
            $scope.warningMessage = msg;
            $scope.hideAlert = false;
            $timeout(function () {
                $scope.hideAlert = true;
            }, 4000);
        });
        clientPushHubProxy.on('showWinner', function (game) {
            fnCancelGameEndCountDown();
            $scope.record = game.Records[game.Records.length - 1];
            $("#showWinnerModal").modal('show');
        });
        clientPushHubProxy.on('showNoWinner', function (game) {
            $scope.record = game.Records[game.Records.length - 1];
            $scope.hideGameEndCountdown = false;
            $scope.gameEndCountdown = 10;
            $scope.onTimeout = function () {
                $scope.tenSectimeout = $timeout($scope.onTimeout, 1000);
                $scope.gameEndCountdown--;
            }
            $scope.tenSectimeout = $timeout($scope.onTimeout, 1000);

            if ($scope.gameEndCountdown <= 0) {
                $timeout.cancel($scope.tenSectimeout);
                $scope.onTimeout = null
            }
            $scope.gameEndTimeout = $timeout(function () {
                $("#showNoWinnerModal").modal('show');
                $scope.hideGameEndCountdown = true;
            }, 50000);
        });
    }
]);





