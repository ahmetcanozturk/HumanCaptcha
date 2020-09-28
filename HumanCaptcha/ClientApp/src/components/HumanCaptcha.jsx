import React, { Component } from 'react';
import { Alert, Button } from 'reactstrap';
import Config from './settings/config';

const API = Config.apiURL;
const QUERY = "captcha";
const defaultSize = 6;

class HumanCaptcha extends Component {
    constructor(props) {
        super(props);

        this.state = { id: "", url: "", size: defaultSize, value: "", token: "", result: null };

        this.handleRefresh = this.handleRefresh.bind(this);
        this.handleValidate = this.handleValidate.bind(this);
    }

    componentDidMount() {
        var size = this.props.size;
        if (!size)
            size = defaultSize;
        this.getCaptcha(size);
    }

    handleRefresh() {
        var size = this.state.size;
        this.getCaptcha(size);
    }

    handleValue = event => {
        const input = event.target;
        this.setState({ value: input.value.toUpperCase() });
    };

    handleValidate(event) {
        event.preventDefault();
        const id = this.state.id;
        const value = this.state.value;

        this.validateCaptcha(id, value, () => {
            if (this.props.onChange && this.state.token) {
                this.props.onChange(this.state.token);
            }
        });
    }

    convertToImage(base64Data, contentType) {
        var sliceSize = 512;
        const byteCharacters = atob(base64Data);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);

            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        return new Blob(byteArrays, { type: contentType });
    }

    render() {
        const { url, value, result } = this.state;
        let alertVisible = false;
        if (result != null)
            alertVisible = !result;
        return (
            <div className="d-flex flex-column">
                <div className="mb-2">
                    <div className="d-flex flex-row">
                        <div className="mr-2">
                            <img src={url} alt="human captcha" title="human captcha" />
                        </div>
                        <div className="pt-2">
                            <Button color="primary" onClick={this.handleRefresh} className="btn-refresh" disabled={result}></Button>
                        </div>
                    </div>
                </div>
                <div>
                    <div className="d-flex flex-row">
                        <div className="mr-2">
                            <input type="text" id="txtValue" name="txtValue" placeholder="Enter captcha value" value={value} onChange={this.handleValue} className="form-control" disabled={result} autoComplete="off" />
                        </div>
                        <div className="mr-2">
                            <Button color="primary" onClick={this.handleValidate} className="btn-validate" disabled={result}>I am not robot</Button>
                        </div>
                        <div>
                            <Alert color="warning" isOpen={alertVisible}>You entered an invalid value!</Alert>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    async getCaptcha(size) {
        var id = "";
        var url = "";
        try {
            const response = await fetch(API + QUERY + "/" + size);
            const data = await response.json();
            id = data.id;
            const blob = this.convertToImage(data.data, data.mimeType);
            url = URL.createObjectURL(blob);
        }
        catch (error) {
            console.log(error);
        }
        this.setState({ id: id, url: url, size: size });
    }

    async validateCaptcha(id, value, callback) {
        let url = API + QUERY;
        let data = { id: id, value: value };
        var result = false;
        var token = "";
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            });
            const resultData = await response.json();
            if (resultData) {
                result = resultData.result;
                token = resultData.token;
            }
        }
        catch (error) {
            console.log(error);
        }
        this.setState({ token: token, result: result });
        callback();
    }
}

export default HumanCaptcha;