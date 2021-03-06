function Game(){
	var canvas = document.getElementById("game");
	this.width=canvas.width;
	this.height=canvas.height;
	this.context = canvas.getContext("2d");
	this.context.fillStyle="white";
}


Game.prototype.draw = function()
{
	this.context.clearRect(0,0,this.width,this.height);
	this.context.fillRect(this.width/2,0,2,this.height);
};

Game.prototype.update = function(){
	if(this.paused)
		return;
};

/* initialize the game instance */

var game = new Game();



function MainLoop(){
	game.update();
	game.draw();
	/*call the mainloop again at a frame rate of 30fps*/
	setTimeout(MainLoop, 33.3333);
}

/* start the game execution */


MainLoop();