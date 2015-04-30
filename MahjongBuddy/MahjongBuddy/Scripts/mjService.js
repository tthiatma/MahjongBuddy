app.factory('mjService', function () {

    var mjServiceFactory = {
        currentPlayer: null,
        topPlayer: null,
        rightPlayer: null,
        leftPlayer: null
    };

    mjServiceFactory.setPlayerPosition = function(game, loc, curPlayerWind)
    {
        //0:east 1:west 2:south 3:north
        if (loc == 'top')
        {
            switch (curPlayerWind) {
                case 0:
                    mjServiceFactory.topPlayer = mjServiceFactory.getPlayerByWind(game, 1);
                    break;
                case 1:
                    mjServiceFactory.topPlayer = mjServiceFactory.getPlayerByWind(game, 0);
                    break;
                case 2:
                    mjServiceFactory.topPlayer = mjServiceFactory.getPlayerByWind(game, 3);
                    break;
                case 3:
                    mjServiceFactory.topPlayer = mjServiceFactory.getPlayerByWind(game, 2);
                    break;
            }
        }
        else if (loc == 'left') {
            switch (curPlayerWind) {
                case 0:
                    mjServiceFactory.leftPlayer = mjServiceFactory.getPlayerByWind(game, 3);
                    break;
                case 1:
                    mjServiceFactory.leftPlayer = mjServiceFactory.getPlayerByWind(game, 2);
                    break;
                case 2:
                    mjServiceFactory.leftPlayer = mjServiceFactory.getPlayerByWind(game, 0);
                    break;
                case 3:
                    mjServiceFactory.leftPlayer = mjServiceFactory.getPlayerByWind(game, 1);
                    break;
            }
        }
        else if (loc == 'right') {
            switch (curPlayerWind) {
                case 0:
                    mjServiceFactory.rightPlayer = mjServiceFactory.getPlayerByWind(game, 2);
                    break;
                case 1:
                    mjServiceFactory.rightPlayer = mjServiceFactory.getPlayerByWind(game, 3);
                    break;
                case 2:
                    mjServiceFactory.rightPlayer = mjServiceFactory.getPlayerByWind(game, 1);
                    break;
                case 3:
                    mjServiceFactory.rightPlayer = mjServiceFactory.getPlayerByWind(game, 0);
                    break;
            }
        }
    }

    mjServiceFactory.setPlayer = function (game, curUserId) {

        //determine current player
        if (game.Player1.ConnectionId == curUserId) {
            mjServiceFactory.currentPlayer = game.Player1;
        }
        else if (game.Player2.ConnectionId == curUserId) {
            mjServiceFactory.currentPlayer = game.Player2;
        }
        else if (game.Player3.ConnectionId == curUserId) {
            mjServiceFactory.currentPlayer = game.Player3;
        }
        else if (game.Player4.ConnectionId == curUserId) {
            mjServiceFactory.currentPlayer = game.Player4;
        }

        //0:east 1:west 2:south 3:north
        for (i = 0; 1 < 4; i++)
        {
            if (mjServiceFactory.currentPlayer.Wind == i) {
                mjServiceFactory.setPlayerPosition(game, 'top', i);
                mjServiceFactory.setPlayerPosition(game, 'left', i);
                mjServiceFactory.setPlayerPosition(game, 'right', i);
                break;
            }
        }
    }

    mjServiceFactory.getPlayerByWind = function (game, wind) {
        if (game.Player1.Wind == wind) {
            return game.Player1;
        }
        else if (game.Player2.Wind == wind) {
            return game.Player2;
        }
        else if (game.Player3.Wind == wind) {
            return game.Player3;
        }
        else if (game.Player4.Wind == wind) {
            return game.Player4;
        }
    }

    mjServiceFactory.getCurrentPlayer = function (game, curUserId) {
        if (game.Player1.ConnectionId == curUserId) {
            return game.Player1;
        }
        else if (game.Player2.ConnectionId == curUserId) {
            return game.Player2;
        }
        else if (game.Player3.ConnectionId == curUserId) {
            return game.Player3;
        }
        else if (game.Player4.ConnectionId == curUserId) {
            return game.Player4;
        }
    }

    mjServiceFactory.getWindName = function (windDirection) {
        switch (windDirection) {
            case 0:
                return 'East';
                break;
            case 1:
                return 'West';
                break;
            case 2:
                return 'South';
                break;
            case 3:
                return 'North';
                break;
        }
    }

    return mjServiceFactory;

});