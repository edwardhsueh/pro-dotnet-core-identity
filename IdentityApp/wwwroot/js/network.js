const baseUrl = "https://localhost:44350/api/data";
const authUrl = "https://localhost:44350/api/auth";
const baseRequestConfig = {
    credentials: "include"
}
export const signIn = async function (email, password, callback, errorHandler) {
    const response = await fetch(`${authUrl}/signin`, {
        ...baseRequestConfig,
        method: "POST",
        body: JSON.stringify({ email, password }),
        headers: { "Content-Type": "application/json" }
    });
    processResponse(response, async () =>
        callback(await response.json()), errorHandler);
}
export const signOut = async function (callback) {
    const response = await fetch(`${authUrl}/signout`, {
        ...baseRequestConfig,
        method: "POST"
    });
    processResponse(response, callback, callback);
}
export const loadData = async function (callback, errorHandler) {
    console.log("loadData 1:"+baseUrl);

    const response = await fetch(baseUrl, {
        ...baseRequestConfig,
        redirect: "manual"
    });
    let result = response;
    console.log("loadData 2:"+JSON.stringify(result));
//     let result = await response.json();
//     console.log("loadData response:"+JSON.stringify(result));
//     processResponse(response, async () =>
//         callback(result), errorHandler);
}
export const createProduct = async function (product, callback, errorHandler) {
    const response = await fetch(baseUrl, {
        ...baseRequestConfig,
        method: "POST",
        body: JSON.stringify(product),
        headers: {
            "Content-Type": "application/json"
        }
    });
    processResponse(response, callback, errorHandler);
}
export const deleteProduct = async function (id, callback, errorHandler) {
    const response = await fetch(`${baseUrl}/${id}`, {
        ...baseRequestConfig,
        method: "DELETE"
    });
    processResponse(response, callback, errorHandler);
}
function processResponse(response, callback, errorHandler) {
    if (response.ok) {
        callback();
    } else {
        errorHandler(response.status);
    }
}