# Chester Devs Website 

This repository is the source to the [Chester Devs website](https://chester.dev/)

This project is open to [contributing](CONTRIBUTING.md), including re-writing it in a different language. 

# Hopes and dreams

There are many enhancements I'd like to see for this site and I'm would love to see more ideas submitted.

See the [enhancement issues](https://github.com/ChesterDevs/chester-devs-website/labels/enhancement) for the list.

# Build

The initial build is using the following, but only because that's what I'm most familiar with and so the fastest way to get the ball rolling.

- Asp.Net Core
- Bootstrap with a theme

Please fork this repo, add features, add copies of the site in some other framework, tweak and improve where you see fit and submit a pull request. For details see the [contributing](CONTRIBUTING.md) page.

## Secrets

We now need to store api keys etc for some of the website features. These are stored in a file called info.json (./ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet/info.json), but to keep them secret this file is added to the .gitignore file and re-added via the build pipeline.

To work locally recreate this file from the template (./ChesterDevs.Core.Aspnet/ChesterDevs.Core.Aspnet/app/secrets/template.json) and add your own api keys. 

|Key name     | Instructions  |
|-------------|---------------|
|GoogleApiKey | [Google Api Key for YouTube](https://support.google.com/googleapi/answer/6158862?hl=en)|
