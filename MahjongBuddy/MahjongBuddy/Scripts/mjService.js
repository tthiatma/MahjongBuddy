app.factory('mjService', function () {

    var mjServiceFactory = {
        currentPlayer: null
    };

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
                return 'South';
                break;
            case 2:
                return 'West';
                break;
            case 3:
                return 'North';
                break;
        }
    }

    return mjServiceFactory;

});