export PROJECT_NAMESPACE=${PROJECT_NAMESPACE:-05e933}
export GIT_URI=${GIT_URI:-"https://github.com/bcgov/gcpe-hub-api.git"}
export GIT_REF=${GIT_REF:-"develop"}

# The project components
# - They are all contained under the main OpenShift folder.
export components=${components:-"."}

# The templates that should not have their GIT referances(uri and ref) over-ridden
# Templates NOT in this list will have they GIT referances over-ridden
# with the values of GIT_URI and GIT_REF
export skip_git_overrides=""

# The builds to be triggered after buildconfigs created (not auto-triggered)
export builds=${builds:-""}

# The images to be tagged after build
export images=${images:-"hub-api"}

# The routes for the project
export routes=${routes:-"hub-api"}