// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

var path = require("path");

module.exports = {
    mode: "development",
    entry: {
        "app": "./src/Client/App/App.fsproj",
        "auth": "./src/Client/Auth/Auth.fsproj",
    },
    output: {
        path: path.join(__dirname, "./public"),
        filename: "[name].js",
    },
    devServer: {
        publicPath: "/",
        contentBase: "./public",
        port: 5003,
        https: true,
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }]
    }
}