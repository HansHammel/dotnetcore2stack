npm install tsd -g
npm install typings -g

npm install npm@latest
npm init -y
npm install webpack --save-dev
npm install --save-dev typescript ts-loader
npm install --save-dev knockout @types/knockout
tsconfig.json
{
  "compilerOptions": {
    "outDir": "./wwwroot/build/",
    "noImplicitAny": false,
    "noEmitOnError": true,
    "removeComments": false,
    "sourceMap": true,
    "target": "es5",
    "module": "commonjs",
    "moduleResolution": "node",
    "compileOnSave": true
  },
  "exclude": [
    "node_modules",
    "wwwroot"
  ]
}

scripts/myviewmodel.ts
import * as ko from "knockout"

class MyViewModel {
    firstname: KnockoutObservable<string>;
    lastname: KnockoutObservable<string>;

    constructor(firstname: string, lastname: string) {
        this.firstname = ko.observable(firstname);
        this.lastname = ko.observable(lastname);
    }
}

ko.applyBindings(new MyViewModel("Jakob", "Christensen"));


webpack.config.js 
var path = require('path');

module.exports = {
    entry: {
        site: [
            './wwwroot/js/site.js', 
            './scripts/myviewmodel.ts']
    },
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, 'wwwroot/dist/')
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                loader: 'ts-loader',
                exclude: /node_modules/,
            },
        ]
    },
    resolve: {
        extensions: [".tsx", ".ts", ".js"]
    }
};

Then remove the line containing “compileOnSave” from tsconfig.json.

package.json - for task runner (not recommanded use prebuild instead)
"scripts": {
    "build": "webpack"
  },
"-vs-binding":{"BeforeBuild":["build"]}

prebuild:
npm run build

your project name].csproj
<PropertyGroup>
    <!-- ... -->
    <TypeScriptCompileBlocked>true</TypeScriptCompileBlocked>
</PropertyGroup>

Also, you might want to remove bower.json and bundleconfig.json if present, as package.json and webpack.config.js replace them. 
