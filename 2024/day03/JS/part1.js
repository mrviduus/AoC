const fs = require('fs');
const path = require('path');

const INPUT_FILE = path.join(__dirname, '../input.txt');

function readInputFile(filePath){
    try{
        return fs.readFileSync(filePath, 'utf8').split(`\n`);
    }catch(e){
        console.error(`error while reading file from path: ${filePath}`);
    }
}

function mulRegex() {
    return /mul\((\d+),(\d+)\)/g;
  }
  
  function calculateSum(input) {
    let sum = input.reduce((acc, line) => {
      let match;
      const regex = mulRegex();
      while ((match = regex.exec(line)) !== null) {
        const x = parseInt(match[1], 10);
        const y = parseInt(match[2], 10);
        acc += x * y;
      }
      return acc;
    }, 0);
  
    return sum.toString();
  }
// Main execution

const input = readInputFile(INPUT_FILE);

console.log(calculateSum(input));
