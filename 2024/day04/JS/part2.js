const fs = require('fs');
const path = require('path');

const INPUT_FILE = path.join(__dirname, '../input.txt');

const input = fs.readFileSync(INPUT_FILE, 'utf8').split('\n');

const rows = input.length;
const columns = input[0].length;

let count = 0;
for(let i = 1; i < rows -1; i ++){
    for(let j = 1; j < columns -1; j ++){

        if(input[i][j] != 'A') continue;

        // Check upper-left + lower-right diagonal combined with upper-right + lower-left diagonal
        var topLeftMAS = i > 0 && j > 0 && input[i-1][j-1] == 'M' && input[i+1][j+1] == 'S';
        var topLeftSAM = i > 0 && j > 0 && input[i-1][j-1] == 'S' && input[i+1][j+1] == 'M';
        var topRightMAS = i > 0 && j < columns-1 && input[i-1][j+1] == 'M' && input[i+1][j-1] == 'S';
        var topRightSAM = i > 0 && j < columns-1 && input[i-1][j+1] == 'S' && input[i+1][j-1] == 'M'

        if ((topLeftMAS || topLeftSAM) && (topRightMAS || topRightSAM))
        {
            count++;
        }
    }
}

console.log(`count: ${count}`);