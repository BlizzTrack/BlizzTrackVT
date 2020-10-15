var path = require('path');

module.exports = {
    entry: {
        main: './Scripts/main.ts'
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },

    output: {
        path: path.join(__dirname, '/wwwroot/js/'),
        filename: 'main.build.js'
    }
};