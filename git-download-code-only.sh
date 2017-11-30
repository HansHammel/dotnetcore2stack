#mkdir dotnetcore2stack
#cd dotnetcore2stack
#git init
#git config core.sparseCheckout true
#echo "!.idea/*" > .git/info/sparse-checkout
#echo "!.idea_modules/*" > .git/info/sparse-checkout
#echo "/*" > .git/info/sparse-checkout
#git remote add origin https://github.com/HansHammel/dotnetcore2stack.git
#git pull --depth=1 origin master

#shorter
rm -rf dotnetcore2stack
git clone --depth=1 --no-checkout https://github.com/HansHammel/dotnetcore2stack.git
cd dotnetcore2stack
git config core.sparsecheckout true
echo "!Tools/*" >> .git/info/sparse-checkout
echo "!Samples/*" >> .git/info/sparse-checkout
echo "!NoneDotNetCore/*" >> .git/info/sparse-checkout
echo "!Snippets/*" >> .git/info/sparse-checkout
echo "/*" >> .git/info/sparse-checkout
git checkout master
cd ..
