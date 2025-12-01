const fs = require('fs');
const path = require('path');

const INPUT_FILE = path.join(__dirname, '../input.txt');

function isOrdered(update, rules) {
    for (let i = 0; i < update.length - 1; i++) {
        if (!rules.find((rule) => rule[0] === update[i] && rule[1] === update[i + 1])) {
            return false;
        }
    }
    return true;
}

function calculate(updates, rules) {
    const result = updates.reduce((acc, update) => {
        if (!isOrdered(update, rules)) return acc;

        const midIndex = Math.floor(update.length / 2);
        const mid = parseInt(update[midIndex]);
        acc += isNaN(mid) ? 0 : mid;
        return acc;

    }, 0);
    return result;
}

// Main execution

let [rules, updates] = fs.readFileSync(INPUT_FILE, 'utf-8').split('\n\n');
rules = rules.split('\n').map(rule => rule.split("|"));
updates = updates.split('\n').map(update => update.split(","));

var result = calculate(updates, rules);

console.log({ result });
