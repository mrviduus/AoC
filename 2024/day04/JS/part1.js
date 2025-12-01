const fs = require('fs');
const path = require('path');

const INPUT_FILE = path.join(__dirname, '../input.txt');

const TARGET_WORD = 'XMAS';


function countAll(input, rows, columns) {
    let sum = 0;

    for (let i = 0; i < rows; i++) {
        for (let j = 0; j < columns; j++) {
            sum += countAllFromPoint(i, j);
        }
    }

    return sum.toString();

    function countAllFromPoint(i, j) {
        if (input[i][j] !== 'X') return 0;

        let count = 0;

        // up
        if (i - 3 >= 0 && input[i - 1][j] === 'M' && input[i - 2][j] === 'A' && input[i - 3][j] === 'S')
            count++;

        // down
        if (i + 3 < rows && input[i + 1][j] === 'M' && input[i + 2][j] === 'A' && input[i + 3][j] === 'S')
            count++;

        // left
        if (j - 3 >= 0 && input[i][j - 1] === 'M' && input[i][j - 2] === 'A' && input[i][j - 3] === 'S')
            count++;

        // right
        if (j + 3 < columns && input[i][j + 1] === 'M' && input[i][j + 2] === 'A' && input[i][j + 3] === 'S')
            count++;

        // up-left
        if (i - 3 >= 0 && j - 3 >= 0 && input[i - 1][j - 1] === 'M' && input[i - 2][j - 2] === 'A' && input[i - 3][j - 3] === 'S')
            count++;

        // up-right
        if (i - 3 >= 0 && j + 3 < columns && input[i - 1][j + 1] === 'M' && input[i - 2][j + 2] === 'A' && input[i - 3][j + 3] === 'S')
            count++;

        // down-left
        if (i + 3 < rows && j - 3 >= 0 && input[i + 1][j - 1] === 'M' && input[i + 2][j - 2] === 'A' && input[i + 3][j - 3] === 'S')
            count++;

        // down-right
        if (i + 3 < rows && j + 3 < columns && input[i + 1][j + 1] === 'M' && input[i + 2][j + 2] === 'A' && input[i + 3][j + 3] === 'S')
            count++;

        return count;
    }
}
//Main execution

const input = fs.readFileSync(INPUT_FILE, 'utf8').split('\n');

const rows = input.length;
const columns = input[0].length;
//console.log(`rows: ${rows}`);
//console.log(`colums: ${colums}`);


let sum = countAll(input, rows, columns);

console.log(`sum: ${sum}`);





